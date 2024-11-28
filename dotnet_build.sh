#!/bin/bash
set -euo pipefail

# Clean up unused Docker resources
docker system prune -f

# Create the temporary directory
mkdir -p tempdir

# Sync files, excluding unnecessary ones
rsync -av --delete \
  --exclude 'bin/' \
  --exclude 'obj/' \
  --exclude 'README' \
  --exclude '.gitignore' \
  --exclude '.git/' \
  ./ tempdir/

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
RUN dotnet restore "Rise.Server/Rise.Server.csproj"
RUN dotnet restore "Rise.Persistence/Rise.Persistence.csproj"

# Copy remaining files
COPY . .

# Change repository
WORKDIR "/app/Rise.Server"

# Build the application
RUN dotnet build "Rise.Server.csproj" -c Release -o /app/build

WORKDIR "/app"

# Install dotnet tools in build image
RUN dotnet tool install --global dotnet-ef

# Add tools path to environment
ENV PATH="\${PATH}:/root/.dotnet/tools"

# Apply database migrations
RUN dotnet-ef database update --startup-project Rise.Server --project Rise.Persistence

# Run tests during the build phase
WORKDIR "/app"
RUN dotnet test "Rise.Client.Tests/Rise.Client.Tests.csproj" --no-build --verbosity normal
RUN dotnet test "Rise.Domain.Tests/Rise.Domain.Tests.csproj" --no-build --verbosity normal
RUN dotnet test "Rise.PlaywrightTests/Rise.PlaywrightTests.csproj" --no-build --verbosity normal
RUN dotnet test "Rise.Server.Tests/Rise.Server.Tests.csproj" --no-build --verbosity normal

# Publish the application
WORKDIR "/app/Rise.Server"
RUN dotnet publish "Rise.Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app

# Copy the published files
COPY --from=build /app/publish .

# Add tools path to environment
ENV PATH="\${PATH}:/root/.dotnet/tools"
ENV ASPNETCORE_URLS=http://+:5000

# Start the application
ENTRYPOINT ["dotnet", "Rise.Server.dll"]
_EOF_

# Build the Docker image, tagging it with the current Git commit hash for versioning
GIT_COMMIT_HASH=$(git rev-parse --short HEAD)
docker build -t dotnet:$GIT_COMMIT_HASH tempdir

# Stop and remove any running container with the same name
if docker ps -a --filter "name=dotnetapp" --format '{{.Names}}' | grep -q dotnetapp; then
  docker rm -f dotnetapp
fi

# Run the new container
docker run -t -d -p 5000:5000 --name dotnetapp dotnet:$GIT_COMMIT_HASH

# List the running Docker containers
docker ps -a | grep
