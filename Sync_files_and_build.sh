#!/bin/bash
set -euo pipefail

# Clean up unused Docker resources
docker system prune -f

# Define workspace and temp directory paths
WORKSPACE_DIR="$(pwd)"
TEMP_DIR="$WORKSPACE_DIR/tempdir"

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

# Build the Docker image, from the dockerfile in the tempdir
GIT_COMMIT_HASH=$(git rev-parse --short HEAD)
docker build -t dotnet:$GIT_COMMIT_HASH tempdir

# Stop and remove any running container with the same name
if docker ps -a --filter "name=dotnetapp" --format '{{.Names}}' | grep -q dotnetapp; then
  docker rm -f dotnetapp
fi

# Run the new container
docker run -t -d -p 6000:5000 --name dotnetapp dotnet:$GIT_COMMIT_HASH

# List the running Docker containers
docker ps -a | grep dotnetapp
