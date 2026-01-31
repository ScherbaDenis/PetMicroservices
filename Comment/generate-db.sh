#!/bin/bash

# Database generation script for Comment microservice
# This script applies EF Core migrations to create/update the database

echo "=== Comment Microservice - Database Generation ==="
echo ""

# Navigate to the DataAccess project directory
cd "$(dirname "$0")/Comment.DataAccess.MsSql"

# Check if EF Core tools are installed
if ! dotnet ef --version > /dev/null 2>&1; then
    echo "Entity Framework Core tools not found. Installing..."
    dotnet tool install --global dotnet-ef
    echo ""
fi

# Restore packages first
echo "Restoring packages..."
dotnet restore ../WebApiComment

# Apply migrations to create/update the database
echo "Applying migrations to the database..."
dotnet ef database update --startup-project ../WebApiComment

if [ $? -eq 0 ]; then
    echo ""
    echo "✓ Database successfully created/updated!"
    echo ""
    echo "Connection string: Server=localhost;Database=CommentDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
else
    echo ""
    echo "✗ Failed to update database. Please check the error messages above."
    exit 1
fi
