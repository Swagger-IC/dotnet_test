#!/bin/bash
GIT_COMMIT_HASH=$(git rev-parse --short HEAD)
remote_server="192.168.56.13"
SSH_connection="root@$remote_server"

# save the container to a tarball
docker save dotnet:$GIT_COMMIT_HASH > dotnet.tar

#Transfer the tarball to the remote server
scp dotnet.tar $SSH_connection:/root/

#unpack the tarball
ssh $SSH_connection "docker load < /root/dotnet.tar" || {
    echo "Failed to load Docker image on remote server."
    exit 1
}

# remove the tarball
ssh $SSH_connection "rm -f /root/dotnet.tar"
