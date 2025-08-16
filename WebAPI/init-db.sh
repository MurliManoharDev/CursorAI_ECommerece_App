#!/bin/bash
# Database initialization script for Azure Linux App Service

echo "Initializing database..."

# Check if database exists in /home
if [ ! -f "/home/ecommerce.db" ]; then
    echo "Database not found at /home/ecommerce.db"
    
    # Try to copy from app directory
    if [ -f "/home/site/wwwroot/ecommerce.db" ]; then
        echo "Copying database from wwwroot to /home..."
        cp "/home/site/wwwroot/ecommerce.db" "/home/ecommerce.db"
        chmod 666 "/home/ecommerce.db"
        echo "Database copied and permissions set"
    else
        echo "Warning: No database file found to copy"
    fi
else
    echo "Database already exists at /home/ecommerce.db"
fi

# Verify database
if [ -f "/home/ecommerce.db" ]; then
    echo "Database verified at /home/ecommerce.db"
    ls -la "/home/ecommerce.db"
else
    echo "ERROR: Database setup failed"
fi 