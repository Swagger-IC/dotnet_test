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
GIT_COMMIT_HASH=$(git rev-parse --short HEAD)
FULL_IMAGE_NAME="$IMAGE_NAME:$GIT_COMMIT_HASH"
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

# Build the Docker image, tagging it with the current Git commit hash for versioning
docker build -t dotnet:$GIT_COMMIT_HASH tempdir

# Deploy or update the service
if docker service ls | grep -q $SERVICE_NAME; then
  echo "Updating the service to use git commit hash $GIT_COMMIT_HASH"
  docker service update --image $FULL_IMAGE_NAME $SERVICE_NAME
else
  echo "Creating the service with git commit hash $GIT_COMMIT_HASH"
  docker service create --name $SERVICE_NAME --replicas 1 --publish 5000:5000 $FULL_IMAGE_NAME
fi

# Wait for 15 seconds for the service to start
sleep 15

# Clean up unused Docker resources
docker system prune -f

# List the running Docker services
docker service ls
