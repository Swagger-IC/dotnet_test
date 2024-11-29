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
IMAGE_VERSION=$(date +"%Y%m%d%H%M%S") # Timestamp for unique tagging
FULL_IMAGE="$IMAGE_NAME:$IMAGE_VERSION"

# Create the temporary directory if it doesn't exist
if [ ! -d tempdir ]; then
  mkdir -p tempdir
fi

# Sync files, excluding unnecessary ones
rsync -av --delete \
  --exclude 'bin/' \
  --exclude 'obj/' \
  --exclude 'README.md' \
  --exclude '.gitignore' \
  --exclude '.git/' \
  --exclude 'dotnet_tests.sh' \
  ./ tempdir/

# Copy the content of the folders
declare -a folders=("Rise.Client" "Rise.Client.Tests" "Rise.Domain" "Rise.Domain.Tests" "Rise.Persistence" "Rise.PlaywrightTests" "Rise.Server" "Rise.Server.Tests" "Rise.Services" "Rise.Shared")

for folder in "${folders[@]}"; do
  echo "Copying $folder files"
  cp -r "$folder"/* "tempdir/$folder"
done

# Copy the solution file to tempdir
cp Rise.sln tempdir

sed -i 's|^\s*"SqlServer": *".*"|    "SqlServer": "Server=192.168.56.11,1433;Database=Hogent.RiseDb;User Id=sa;Password=Password1234!;Encrypt=Optional;TrustServerCertificate=true"|' tempdir/Rise.Server/appsettings.json

# Create the Dockerfile
cat > tempdir/Dockerfile << _EOF_
# Use aspnet for .NET runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

EXPOSE 5000

# Use the official .NET 8 SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /app

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

RUN dotnet restore "Rise.Server/Rise.Server.csproj"
RUN dotnet restore "Rise.Persistence/Rise.Persistence.csproj"

COPY . .

WORKDIR "/app/Rise.Server"

RUN dotnet build "Rise.Server.csproj" -c Release -o /app/build

WORKDIR "/app"

RUN dotnet tool install --global dotnet-ef

ENV PATH="${PATH}:/root/.dotnet/tools"

RUN dotnet-ef database update --startup-project Rise.Server --project Rise.Persistence

WORKDIR "/app/Rise.Server"

RUN dotnet publish "Rise.Server.csproj" -c Release -o /app/publish

FROM base AS FINAL
WORKDIR /app

COPY --from=build /app/publish .

ENV PATH="${PATH}:/root/.dotnet/tools"
ENV ASPNETCORE_URLS=http://+:5000

ENTRYPOINT ["dotnet", "Rise.Server.dll"]
_EOF_

# Navigate to tempdir
cd tempdir || exit

# Build the Docker image with a unique tag
docker build -t $FULL_IMAGE .

# Deploy or update the service
if docker service ls --format '{{.Name}}' | grep -q "^$SERVICE_NAME\$"; then
  echo "Updating existing service $SERVICE_NAME with image $FULL_IMAGE..."
  docker service update --image $FULL_IMAGE $SERVICE_NAME
else
  echo "Creating new service $SERVICE_NAME with image $FULL_IMAGE..."
  docker service create --name $SERVICE_NAME --publish 5000:5000 --replicas 1 $FULL_IMAGE
fi

# Wait for 15 seconds for the service to start
sleep 15

# Clean up unused Docker resources
docker system prune -f

# List the running Docker services
docker service ls

# List the docker containers
docker ps -a
