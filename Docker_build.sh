#!/bin/bash
# Build the Docker image, tagging it with the current Git commit hash for versioning
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
