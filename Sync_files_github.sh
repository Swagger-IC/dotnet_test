#!/bin/bash
set -euo pipefail

# Clean up unused Docker resources
docker system prune -f

# Create the temporary directory if it doesn't exist
if [ ! -d tempdir ]; then
  mkdir -p tempdir
fi

# Sync files, excluding unnecessary ones
rsync -av --delete \
  --exclude 'bin/' \
  --exclude 'obj/' \
  --exclude 'README.md' \
  --exclude '.gitignore' \
  --exclude '.git/' \
  --exclude '*.sh' \
  ./ tempdir/