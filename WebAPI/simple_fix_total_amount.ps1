# Simple PowerShell script to fix total_amount NULL values

Write-Host "Updating NULL total_amount values to 0..." -ForegroundColor Green

# Create SQL file
$sqlContent = "UPDATE orders SET total_amount = 0 WHERE total_amount IS NULL;"
$sqlFile = "temp_update_total_amount.sql"

# Write SQL to file
$sqlContent | Out-File -FilePath $sqlFile -Encoding UTF8

# Execute SQL file
Write-Host "Executing SQL update..."
& dotnet run --project ../SeedDatabase/ExecuteSqlFile.csproj $sqlFile ../WebAPI/ecommerce.db

# Check if successful
if ($LASTEXITCODE -eq 0) {
    Write-Host "Successfully updated total_amount values!" -ForegroundColor Green
} else {
    Write-Host "No rows to update or there was an error." -ForegroundColor Yellow
}

# Clean up temp file
Remove-Item $sqlFile -Force -ErrorAction SilentlyContinue

Write-Host "Done!" -ForegroundColor Green 