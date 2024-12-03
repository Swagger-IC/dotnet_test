#!/bin/bash
set -euo pipefail

GIT_COMMIT_HASH=$(git rev-parse --short HEAD)
interface="eth1"
SERVICE_NAME="dotnetapp"
IMAGE_NAME="dotnet"
FULL_IMAGE="$IMAGE_NAME:$GIT_COMMIT_HASH"

# Clean up unused Docker resources
docker system prune -f

# Initialize Docker Swarm (if not initialized already)
if ! docker info | grep -q 'Swarm: active'; then
  docker swarm init --advertise-addr $interface
fi

#unpack the tarball
docker load < /root/dotnet.tar

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

# Remove the tarball
rm /root/dotnet.tar

# List the running Docker services
docker service ls

# List the docker containers
docker ps -a