#!/bin/bash
set -euo pipefail

# Clean up unused Docker resources
docker system prune -f

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
  --exclude '*.sh' \
  --exclude 'Jenkinsfile' \
  ./ tempdir/

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
