# PowerShell script to create a Linux-compatible deployment package with database
Write-Host "Creating Linux-compatible deployment package with database..." -ForegroundColor Green

# Navigate to publish directory
Set-Location "C:\Users\2325185\code_base\e-commerce-app\WebAPI\bin\Release\net8.0\publish"

# Create a temporary directory for the package
$tempDir = "C:\Users\2325185\code_base\e-commerce-app\linux-deploy-temp"
if (Test-Path $tempDir) {
    Remove-Item -Recurse -Force $tempDir
}
New-Item -ItemType Directory -Path $tempDir | Out-Null

Write-Host "Copying files to temporary directory..." -ForegroundColor Yellow

# Copy all files maintaining structure
Get-ChildItem -Path . -Recurse | ForEach-Object {
    $relativePath = $_.FullName.Substring((Get-Location).Path.Length + 1)
    $destPath = Join-Path $tempDir $relativePath
    
    if ($_.PSIsContainer) {
        New-Item -ItemType Directory -Path $destPath -Force | Out-Null
    } else {
        $destDir = Split-Path $destPath -Parent
        if (!(Test-Path $destDir)) {
            New-Item -ItemType Directory -Path $destDir -Force | Out-Null
        }
        Copy-Item $_.FullName -Destination $destPath -Force
    }
}

# Create a startup script that will copy the database to /home on first run
$startupScript = @'
#!/bin/bash
# This script ensures the database is in the correct location

# Check if database exists in /home
if [ ! -f "/home/ecommerce.db" ]; then
    echo "Copying database to /home..."
    # Try to copy from multiple possible locations
    if [ -f "/home/site/wwwroot/ecommerce.db" ]; then
        cp "/home/site/wwwroot/ecommerce.db" "/home/ecommerce.db"
        echo "Database copied from wwwroot"
    elif [ -f "ecommerce.db" ]; then
        cp "ecommerce.db" "/home/ecommerce.db"
        echo "Database copied from current directory"
    else
        echo "Warning: Database file not found"
    fi
    
    # Set permissions
    if [ -f "/home/ecommerce.db" ]; then
        chmod 666 "/home/ecommerce.db"
        echo "Database permissions set"
    fi
else
    echo "Database already exists at /home/ecommerce.db"
fi
'@

# Save the startup script
$startupScript | Out-File -FilePath "$tempDir\init-db.sh" -Encoding UTF8 -NoNewline

Write-Host "Creating tar archive (Linux-compatible)..." -ForegroundColor Yellow

# Change to temp directory
Set-Location $tempDir

# Create tar file (tar preserves Unix paths)
tar -czf "C:\Users\2325185\code_base\e-commerce-app\linux-deploy-with-db.tar.gz" *

# Clean up temp directory
Set-Location "C:\Users\2325185\code_base\e-commerce-app"
Remove-Item -Recurse -Force $tempDir

Write-Host "`nLinux-compatible package created: linux-deploy-with-db.tar.gz" -ForegroundColor Green
Write-Host "This package includes your development database." -ForegroundColor Cyan 