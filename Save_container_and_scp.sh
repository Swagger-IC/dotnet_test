#!/bin/bash
remote_server="192.168.56.13"
SSH_connection="vagrant@$remote_server"

# Validate if the Docker image 'dotnet' exists
if ! sudo docker image inspect dotnet > /dev/null 2>&1; then
    echo "Docker image 'dotnet' does not exist."
    exit 1
fi

# Save the Docker image 'dotnet' to a tarball
sudo docker save dotnet > dotnet.tar
echo "Docker image 'dotnet' saved to dotnet.tar."

# Ensure the remote server is in known_hosts to avoid SSH verification issues
ssh-keyscan -H $remote_server >> ~/.ssh/known_hosts

# Transfer the tarball to the remote server
scp dotnet.tar $SSH_connection:/home/
if [ $? -eq 0 ]; then
    echo "File transfer to $remote_server was successful."
else
    echo "File transfer to $remote_server failed." >&2
    exit 1
fi

# Remove the local tarball after successful transfer
rm dotnet.tar
echo "Local tarball removed."
