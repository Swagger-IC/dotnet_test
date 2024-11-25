#!/bin/bash
set -euo pipefail

#clean up docker
sudo docker system prune -f

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

#copy the content of the folder
declare -a folders=("Rise.Client" "Rise.Client.Tests" "Rise.Domain" "Rise.Domain.Tests" "Rise.Persistence" "Rise.PlaywrightTests" "Rise.Server" "Rise.Server.Tests" "Rise.Services" "Rise.Shared")

for folder in "${folders[@]}"; do
  echo "Copying $folder files"
  cp -r "$folder"/* "tempdir/$folder"
done

#copy the sln file to tempdir
cp Rise.sln tempdir

cat > tempdir/Dockerfile << _EOF_
# Use the official .NET 8 SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set the working directory for the build process
WORKDIR /src

# Copy the .sln file and the project files into the container
COPY ["Rise.sln", "./"]
COPY ["Rise.Server/Rise.Server.csproj", "Rise.Server/"]
COPY ["Rise.Client/Rise.Client.csproj", "Rise.Client/"]
COPY ["Rise.Domain/Rise.Domain.csproj", "Rise.Domain/"]
COPY ["Rise.Persistence/Rise.Persistence.csproj", "Rise.Persistence/"]
COPY ["Rise.Services/Rise.Services.csproj", "Rise.Services/"]
COPY ["Rise.Shared/Rise.Shared.csproj", "Rise.Shared/"]
COPY ["Rise.Client.Tests/Rise.Client.Tests.csproj", "Rise.Client.Tests/"]
COPY ["Rise.Server.Tests/Rise.Server.Tests.csproj", "Rise.Server.Tests/"]
COPY ["Rise.Domain.Tests/Rise.Domain.Tests.csproj", "Rise.Domain.Tests/"]

# Copy the rest of the application files
COPY . .

# Restore the NuGet packages
RUN dotnet restore
RUN dotnet publish -c Release -o out

# Use the official .NET 8 runtime image to create a runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0

# Set the working directory for the application
WORKDIR /app

# Copy the build output from the build image
COPY --from=build /src/out .

# Expose the port 5000 and 5001
EXPOSE 5000
EXPOSE 5001

#set the port for the application
ENV ASPNETCORE_URLS=http://+:5000

# Set the entry point to run the app in development mode
ENTRYPOINT ["dotnet", "Rise.Server.dll"]
_EOF_

cd tempdir || exit
# Build the Docker image, specifying the current directory as the build context
sudo docker build -t dotnet .
sudo docker run -t -d -p 5000:5000 -p 5001:5001 --name dotnetapp dotnet

#remove tempdir
sudo rm -rf tempdir

# List the running Docker containers
sudo docker ps -a