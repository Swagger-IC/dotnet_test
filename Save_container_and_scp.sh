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

# Check if root has SSH keys
if [ ! -f /root/.ssh/id_rsa ]; then
    echo "Root does not have SSH keys. Creating them..."
    # Generate the SSH key pair without a passphrase
    ssh-keygen -t rsa -b 2048 -N "" -f /root/.ssh/id_rsa

    # Ensure the .ssh directory on the remote server exists and has correct permissions
    ssh $SSH_connection "mkdir -p ~/.ssh && chmod 700 ~/.ssh"

    # Append the public key to the remote server's authorized_keys
    cat /root/.ssh/id_rsa.pub | ssh $SSH_connection "cat >> ~/.ssh/authorized_keys && chmod 600 ~/.ssh/authorized_keys"

    echo "SSH key successfully created and transferred to the remote server."
else
    echo "Root already has SSH keys."
fi


# Transfer the tarball to the remote server
scp -v dotnet_$GIT_COMMIT_HASH.tar $SSH_connection:/home/$user/
if [ $? -eq 0 ]; then
    echo "File transfer to $remote_server was successful."
else
    echo "File transfer to $remote_server failed." >&2
    exit 1
fi

# Remove the local tarball after successful transfer
rm dotnet_$GIT_COMMIT_HASH.tar
echo "Local tarball 'dotnet_$GIT_COMMIT_HASH.tar' removed."
