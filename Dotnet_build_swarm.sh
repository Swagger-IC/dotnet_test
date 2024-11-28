#!/bin/bash
set -euo pipefail

# Clean up Docker
docker system prune -f

# Initialize Docker Swarm (if not initialized already)
if ! docker info | grep -q 'Swarm: active'; then
  docker swarm init
fi

mkdir -p tempdir
mkdir -p tempdir/Rise.Client
mkdir -p tempdir/Rise.Client.Tests
mkdir -p tempdir/Rise.Domain
mkdir -p tempdir/Rise.Domain.Tests
mkdir -p tempdir/Rise.Persistence
mkdir -p tempdir/Rise.PlaywrightTests
mkdir -p tempdir/Rise.Server
mkdir -p tempdir/Rise.Server.Tests
mkdir -p tempdir/Rise.Services
mkdir -p tempdir/Rise.Shared

# Copy the content of the folders
declare -a folders=("Rise.Client" "Rise.Client.Tests" "Rise.Domain" "Rise.Domain.Tests" "Rise.Persistence" "Rise.PlaywrightTests" "Rise.Server" "Rise.Server.Tests" "Rise.Services" "Rise.Shared")

for folder in "${folders[@]}"; do
  echo "Copying $folder files"
  cp -r "$folder"/* "tempdir/$folder"
done

# Copy the solution file to tempdir
cp Rise.sln tempdir

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

# Build the Docker image
docker build -t dotnet .

# Create the Swarm service
docker service create --name dotnetapp --publish 5000:5000 dotnet

# Remove the temporary directory
rm -rf tempdir

# List the running Docker services
docker service ls
