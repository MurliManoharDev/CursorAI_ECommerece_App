# PowerShell script to create a Linux-compatible deployment package
Write-Host "Creating Linux-compatible deployment package..." -ForegroundColor Green

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

Write-Host "Creating tar archive (Linux-compatible)..." -ForegroundColor Yellow

# Change to temp directory
Set-Location $tempDir

# Create tar file (tar preserves Unix paths)
tar -czf "C:\Users\2325185\code_base\e-commerce-app\linux-deploy.tar.gz" *

# Clean up temp directory
Set-Location "C:\Users\2325185\code_base\e-commerce-app"
Remove-Item -Recurse -Force $tempDir

Write-Host "`nLinux-compatible package created: linux-deploy.tar.gz" -ForegroundColor Green
Write-Host "Upload this file to Azure Cloud Shell and extract it." -ForegroundColor Cyan 