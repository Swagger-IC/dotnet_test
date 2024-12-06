#!/bin/bash
set -euo pipefail

GIT_COMMIT_HASH=$(git rev-parse --short HEAD)

# Build the Docker image with the latest commit hash
echo "Building Docker image..."
docker build -t dotnet:$GIT_COMMIT_HASH .

# Run Client Tests
echo "Running Client Tests"
docker run --rm -v $PWD:/app -w /app/Rise.Client.Tests dotnet:$GIT_COMMIT_HASH dotnet test --no-build --logger:trx
