# PowerShell script to update total_amount column to have a default value

Write-Host "Updating total_amount column to allow NULL or set default value..." -ForegroundColor Green

# Create SQL file to update existing rows and set default
$sqlContent = @"
-- First, update any existing NULL values to 0
UPDATE orders SET total_amount = 0 WHERE total_amount IS NULL;

-- Drop the total_amount column (if we want to rely on 'total' column instead)
-- Note: In SQLite, we can't drop columns directly, so we'll just ensure it has values
-- For now, let's just ensure all rows have a value
UPDATE orders SET total_amount = COALESCE(total, 0) WHERE total_amount = 0 OR total_amount IS NULL;
"@

$sqlFile = "temp_fix_total_amount.sql"

# Write SQL to file
$sqlContent | Out-File -FilePath $sqlFile -Encoding UTF8

# Execute SQL file
Write-Host "Executing SQL to fix total_amount column..."
& dotnet run --project ../SeedDatabase/ExecuteSqlFile.csproj $sqlFile ../WebAPI/ecommerce.db

# Check if successful
if ($LASTEXITCODE -eq 0) {
    Write-Host "Successfully fixed total_amount column!" -ForegroundColor Green
} else {
    Write-Host "There was an error fixing the column." -ForegroundColor Yellow
}

# Clean up temp file
Remove-Item $sqlFile -Force -ErrorAction SilentlyContinue

Write-Host "Done!" -ForegroundColor Green 