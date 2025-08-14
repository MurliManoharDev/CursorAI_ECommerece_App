# PowerShell script to check orders table structure

Write-Host "Checking current orders table structure..." -ForegroundColor Green

# Create SQL file to get table info
$sqlContent = "PRAGMA table_info(orders);"
$sqlFile = "temp_check_orders.sql"

# Write SQL to file
$sqlContent | Out-File -FilePath $sqlFile -Encoding UTF8

# Execute SQL file
Write-Host "Getting table information..."
& dotnet run --project ../SeedDatabase/ExecuteSqlFile.csproj $sqlFile ../WebAPI/ecommerce.db

# Also get the CREATE statement
$sqlContent2 = "SELECT sql FROM sqlite_master WHERE type='table' AND name='orders';"
$sqlFile2 = "temp_check_orders_create.sql"
$sqlContent2 | Out-File -FilePath $sqlFile2 -Encoding UTF8

Write-Host "`nGetting CREATE TABLE statement..."
& dotnet run --project ../SeedDatabase/ExecuteSqlFile.csproj $sqlFile2 ../WebAPI/ecommerce.db

# Clean up temp files
Remove-Item $sqlFile -Force -ErrorAction SilentlyContinue
Remove-Item $sqlFile2 -Force -ErrorAction SilentlyContinue

Write-Host "`nDone!" -ForegroundColor Green 