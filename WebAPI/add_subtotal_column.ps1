# PowerShell script to add subtotal column to orders table

Write-Host "Adding subtotal column to orders table..." -ForegroundColor Green

# Create SQL file
$sqlContent = "ALTER TABLE orders ADD COLUMN subtotal REAL DEFAULT 0;"
$sqlFile = "temp_add_subtotal.sql"

# Write SQL to file
$sqlContent | Out-File -FilePath $sqlFile -Encoding UTF8

# Execute SQL file
Write-Host "Executing SQL to add subtotal column..."
& dotnet run --project ../SeedDatabase/ExecuteSqlFile.csproj $sqlFile ../WebAPI/ecommerce.db

# Check if successful
if ($LASTEXITCODE -eq 0) {
    Write-Host "Successfully added subtotal column!" -ForegroundColor Green
} else {
    Write-Host "Column may already exist or there was an error." -ForegroundColor Yellow
}

# Clean up temp file
Remove-Item $sqlFile -Force -ErrorAction SilentlyContinue

Write-Host "Done!" -ForegroundColor Green 