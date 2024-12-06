#!/bin/bash
set -euo pipefail

# Clean up unused Docker resources
docker system prune -f

# Initialize Docker Swarm (if not initialized already)
if ! docker info | grep -q 'Swarm: active'; then
  docker swarm init --advertise-addr eth1
fi

# Variables
SERVICE_NAME="dotnetapp"
IMAGE_NAME="dotnet"
GIT_COMMIT_HASH=$(git rev-parse --short HEAD)
FULL_IMAGE_NAME="$IMAGE_NAME:$GIT_COMMIT_HASH"

# Create the temporary directory if it doesn't exist
if [ ! -d tempdir ]; then
  mkdir -p tempdir
fi

# Sync files from the workspace to tempdir, excluding unnecessary files and directories
rsync -av --delete \
  --exclude 'bin/' \
  --exclude 'obj/' \
  --exclude 'README.md' \
  --exclude '.gitignore' \
  --exclude '.git/' \
  --exclude '*.sh' \
  --exclude 'Jenkinsfile' \
  --exclude 'tempdir/' \
  ./ "$TEMP_DIR/"

# Change connection string for sql server
sed -i 's|^\s*"SqlServer": *".*"|    "SqlServer": "Server=192.168.56.11,1433;Database=Hogent.RiseDb;User Id=sa;Password=Password1234!;Encrypt=Optional;TrustServerCertificate=true"|' tempdir/Rise.Server/appsettings.json

# Create the Dockerfile
cat > tempdir/Dockerfile << _EOF_
# Use aspnet for .NET runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

EXPOSE 5000

# Use the official .NET 8 SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set the working directory for the build process
WORKDIR /app

# Copy the .sln file and the project files into the container
COPY ["Rise.sln", "./"]
COPY ["Rise.Client/Rise.Client.csproj", "Rise.Client/"]
COPY ["Rise.Client.Tests/Rise.Client.Tests.csproj", "Rise.Client.Tests/"]
COPY ["Rise.Domain/Rise.Domain.csproj", "Rise.Domain/"]
COPY ["Rise.Domain.Tests/Rise.Domain.Tests.csproj", "Rise.Domain.Tests/"]
COPY ["Rise.Persistence/Rise.Persistence.csproj", "Rise.Persistence/"]
COPY ["Rise.PlaywrightTests/Rise.PlaywrightTests.csproj", "Rise.PlaywrightTests/"]
COPY ["Rise.Server/Rise.Server.csproj", "Rise.Server/"]
COPY ["Rise.Server.Tests/Rise.Server.Tests.csproj", "Rise.Server.Tests/"]
COPY ["Rise.Services/Rise.Services.csproj", "Rise.Services/"]
COPY ["Rise.Shared/Rise.Shared.csproj", "Rise.Shared/"]

# Restore as distinct layers
RUN dotnet restore "Rise.Client/Rise.Client.csproj"
RUN dotnet restore "Rise.Client.Tests/Rise.Client.Tests.csproj"
RUN dotnet restore "Rise.Domain/Rise.Domain.csproj"
RUN dotnet restore "Rise.Domain.Tests/Rise.Domain.Tests.csproj"
RUN dotnet restore "Rise.Persistence/Rise.Persistence.csproj"
RUN dotnet restore "Rise.PlaywrightTests/Rise.PlaywrightTests.csproj"
RUN dotnet restore "Rise.Server/Rise.Server.csproj"
RUN dotnet restore "Rise.Server.Tests/Rise.Server.Tests.csproj"
RUN dotnet restore "Rise.Services/Rise.Services.csproj"
RUN dotnet restore "Rise.Shared/Rise.Shared.csproj"

# Copy remaining files
COPY . .

# Change repository
WORKDIR "/app/Rise.Server"

# Build the application
RUN dotnet build "Rise.Server.csproj" -c Release -o /app/build

WORKDIR "/app"

# dotnet-ef install
RUN dotnet tool install --global dotnet-ef
ENV PATH="${PATH}:/root/.dotnet/tools"
RUN dotnet-ef --version

# Apply database migrations
RUN dotnet-ef database update --startup-project Rise.Server --project Rise.Persistence

# Publish the application as user app
WORKDIR /app/Rise.Server
RUN dotnet publish "Rise.Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app

# Copy the published files
COPY --from=build /app/publish .

# Add tools path to environment
ENV PATH="${PATH}:/root/.dotnet/tools"
ENV ASPNETCORE_URLS=http://+:5000

# Start the application
ENTRYPOINT ["dotnet", "Rise.Server.dll"]
_EOF_

# Build the Docker image, tagging it with the current Git commit hash for versioning
docker build -t dotnet:$GIT_COMMIT_HASH tempdir

# Deploy or update the service
if docker service ls | grep -q $SERVICE_NAME; then
  echo "Updating the service to use git commit hash $GIT_COMMIT_HASH"
  docker service update --image $FULL_IMAGE_NAME $SERVICE_NAME
else
  echo "Creating the service with git commit hash $GIT_COMMIT_HASH"
  docker service create --name $SERVICE_NAME --replicas 1 --publish 5000:5000 $FULL_IMAGE_NAME
fi

# Wait for 15 seconds for the service to start
sleep 15

# Clean up unused Docker resources
docker system prune -f

# List the running Docker services
docker service ls
