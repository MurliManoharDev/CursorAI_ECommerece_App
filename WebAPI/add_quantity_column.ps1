# PowerShell script to add quantity column to order_items table

Write-Host "Adding quantity column to order_items table..." -ForegroundColor Green

# Create SQL file
$sqlContent = "ALTER TABLE order_items ADD COLUMN quantity INTEGER DEFAULT 1;"
$sqlFile = "temp_add_quantity.sql"

# Write SQL to file
$sqlContent | Out-File -FilePath $sqlFile -Encoding UTF8

# Execute SQL file
Write-Host "Executing SQL to add quantity column..."
& dotnet run --project ../SeedDatabase/ExecuteSqlFile.csproj $sqlFile ../WebAPI/ecommerce.db

# Check if successful
if ($LASTEXITCODE -eq 0) {
    Write-Host "Successfully added quantity column!" -ForegroundColor Green
} else {
    Write-Host "Column may already exist or there was an error." -ForegroundColor Yellow
}

# Clean up temp file
Remove-Item $sqlFile -Force -ErrorAction SilentlyContinue

Write-Host "Done!" -ForegroundColor Green 