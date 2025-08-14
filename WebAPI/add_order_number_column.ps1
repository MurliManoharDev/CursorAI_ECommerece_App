# PowerShell script to add order_number column to orders table

Write-Host "Adding order_number column to orders table..." -ForegroundColor Green

# Create SQL file
$sqlContent = "ALTER TABLE orders ADD COLUMN order_number TEXT;"
$sqlFile = "temp_add_order_number.sql"

# Write SQL to file
$sqlContent | Out-File -FilePath $sqlFile -Encoding UTF8

# Execute SQL file
Write-Host "Executing SQL to add order_number column..."
& dotnet run --project ../SeedDatabase/ExecuteSqlFile.csproj $sqlFile ../WebAPI/ecommerce.db

# Check if successful
if ($LASTEXITCODE -eq 0) {
    Write-Host "Successfully added order_number column!" -ForegroundColor Green
} else {
    Write-Host "Column may already exist or there was an error." -ForegroundColor Yellow
}

# Clean up temp file
Remove-Item $sqlFile -Force -ErrorAction SilentlyContinue

Write-Host "Done!" -ForegroundColor Green 