#!/bin/bash
set -euo pipefail

WORKSPACE_DIR="$(pwd)"
TEMP_DIR="$WORKSPACE_DIR/tempdir"
GIT_COMMIT_HASH=$(git rev-parse --short HEAD)

# Check if dotnetapp container is running
if docker ps -a --filter "name=dotnetapp" --format '{{.Names}}' | grep -q dotnetapp; then
  echo "Container dotnetapp is running"
  docker stop dotnetapp
  docker rm dotnetapp
fi

# Clean up unused Docker resources
docker system prune -f

# remove previous dotnet image
docker rmi -f $(docker images -q dotnet) || true

# Ensure the tempdir exists
if [ ! -d "$TEMP_DIR" ]; then
  mkdir -p "$TEMP_DIR"
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


# Create the Dockerfile dynamically in tempdir
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

# Run tests during the build phase
WORKDIR "/app"
#RUN dotnet test "Rise.Client.Tests/"
RUN dotnet test "Rise.Domain.Tests/"
#RUN dotnet test "Rise.PlaywrightTests/"
RUN dotnet test "Rise.Server.Tests/"

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

# Run the new container
docker run -t -d -p 8000:5000 --name dotnetapp dotnet:$GIT_COMMIT_HASH

# List the running Docker containers
docker ps -a | grep dotnetapp
