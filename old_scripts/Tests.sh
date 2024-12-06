#!/bin/bash
set -euo pipefail

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
sed -i 's|^\s*"SqlServer": *".*"|    "SqlServer": "Server=192.168.56.11,1433;Database=Hogent.RiseTestDb;User Id=sa;Password=Password1234!;Encrypt=Optional;TrustServerCertificate=true"|' tempdir/Rise.Server/appsettings.json

# Check if dotnet is installed
if ! command -v dotnet &> /dev/null; then
  echo "dotnet is not installed. Installing dotnet..."
    # Install dotnet
    dnf install -y dotnet-sdk-8.0
fi

# Run tests
dotnet test "$TEMP_DIR/Rise.Server.Tests/Rise.Server.Tests.csproj" --logger "trx;LogFileName=test_results.trx" --results-directory "$TEMP_DIR/TestResults"
