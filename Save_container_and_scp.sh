#!/bin/bash
GIT_COMMIT_HASH=$(git rev-parse --short HEAD)
remote_server="192.168.56.13"
SSH_connection="vagrant@$remote_server"

# save the container to a tarball
docker save dotnet:$GIT_COMMIT_HASH > dotnet.tar

#Transfer the tarball to the remote server
scp -i /var/jenkins_home/.ssh/id_rsa dotnet.tar $SSH_connection:/home/vagrant/

# Remove the tarball
rm dotnet.tar
