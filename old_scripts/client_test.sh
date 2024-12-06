#!/bin/bash
set -euo pipefail

GIT_COMMIT_HASH=$(git rev-parse --short HEAD)

# Run Client Tests
echo "Running Client Tests"
docker run -t -v $PWD:/app -w /app/Rise.Client.Tests dotnet:$GIT_COMMIT_HASH dotnet test --no-build --logger:trx
