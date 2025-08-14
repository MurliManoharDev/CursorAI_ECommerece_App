# PowerShell script to add all missing financial columns to orders table

Write-Host "Adding all financial columns to orders table..." -ForegroundColor Green

$columns = @(
    "subtotal REAL DEFAULT 0",
    "shipping_cost REAL DEFAULT 0", 
    "tax REAL DEFAULT 0",
    "total REAL DEFAULT 0",
    "total_amount REAL DEFAULT 0"
)

Write-Host "Checking and adding missing financial columns..."

foreach ($column in $columns) {
    $columnName = $column.Split(" ")[0]
    $sqlFile = "temp_add_$columnName.sql"
    
    # Create SQL file for this column
    "ALTER TABLE orders ADD COLUMN $column;" | Out-File -FilePath $sqlFile -Encoding UTF8
    
    Write-Host "Checking column: $columnName..." -NoNewline
    
    # Execute SQL file, capture output
    $output = & dotnet run --project ../SeedDatabase/ExecuteSqlFile.csproj $sqlFile ../WebAPI/ecommerce.db 2>&1 | Out-String
    
    if ($output -like "*duplicate column*") {
        Write-Host " Already exists" -ForegroundColor Yellow
    } elseif ($LASTEXITCODE -eq 0) {
        Write-Host " Added successfully!" -ForegroundColor Green
    } else {
        Write-Host " Error" -ForegroundColor Red
    }
    
    # Clean up temp file
    Remove-Item $sqlFile -Force -ErrorAction SilentlyContinue
}

# Now update the total_amount column to copy values from total column
Write-Host "`nUpdating total_amount to match total column values..."
$updateSql = "UPDATE orders SET total_amount = COALESCE(total, subtotal + shipping_cost + tax, 0) WHERE total_amount = 0 OR total_amount IS NULL;"
$updateFile = "temp_update_total_amount.sql"
$updateSql | Out-File -FilePath $updateFile -Encoding UTF8

& dotnet run --project ../SeedDatabase/ExecuteSqlFile.csproj $updateFile ../WebAPI/ecommerce.db
Remove-Item $updateFile -Force -ErrorAction SilentlyContinue

Write-Host "`nFinished adding financial columns!" -ForegroundColor Green 