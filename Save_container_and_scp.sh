#!/bin/bash
remote_server="192.168.56.13"
user="vagrant"
SSH_connection="$user@$remote_server" 
GIT_COMMIT_HASH=$(git rev-parse --short HEAD)

# Validate if the image exists
if ! docker image inspect dotnet:$GIT_COMMIT_HASH > /dev/null 2>&1; then
    echo "Docker image 'dotnet:$GIT_COMMIT_HASH' does not exist after build."
    exit 1
fi

# Save the Docker image to a tarball
docker save dotnet:$GIT_COMMIT_HASH > dotnet_$GIT_COMMIT_HASH.tar
echo "Docker image 'dotnet:$GIT_COMMIT_HASH' saved to dotnet_$GIT_COMMIT_HASH.tar."

# Check if root has ssh keys
if [ ! -f /root/.ssh/id_rsa ]; then
    echo "Root does not have ssh keys. Creating them..."
    ssh-keygen -t rsa -b 2048 -N ""
    ssh-copy-id $SSH_connection
fi

# Transfer the tarball to the remote server
scp -v dotnet_$GIT_COMMIT_HASH.tar $SSH_connection:/home/
if [ $? -eq 0 ]; then
    echo "File transfer to $remote_server was successful."
else
    echo "File transfer to $remote_server failed." >&2
    exit 1
fi

# Remove the local tarball after successful transfer
rm dotnet_$GIT_COMMIT_HASH.tar
echo "Local tarball 'dotnet_$GIT_COMMIT_HASH.tar' removed."
