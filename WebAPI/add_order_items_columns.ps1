# PowerShell script to add missing columns to order_items table

Write-Host "Adding missing columns to order_items table..." -ForegroundColor Green

$columns = @(
    "price REAL DEFAULT 0",
    "total REAL DEFAULT 0",
    "product_name TEXT",
    "product_image TEXT",
    "shipping_type TEXT",
    "shipping_cost REAL DEFAULT 0"
)

Write-Host "Checking and adding missing columns..."

foreach ($column in $columns) {
    $columnName = $column.Split(" ")[0]
    $sqlFile = "temp_add_$columnName.sql"
    
    # Create SQL file for this column
    "ALTER TABLE order_items ADD COLUMN $column;" | Out-File -FilePath $sqlFile -Encoding UTF8
    
    Write-Host "Checking column: $columnName..." -NoNewline
    
    # Execute SQL file, capture output
    $output = & dotnet run --project ../SeedDatabase/ExecuteSqlFile.csproj $sqlFile ../WebAPI/ecommerce.db 2>&1 | Out-String
    
    if ($output -like "*duplicate column*") {
        Write-Host " Already exists" -ForegroundColor Yellow
    } elseif ($LASTEXITCODE -eq 0) {
        Write-Host " Added successfully!" -ForegroundColor Green
    } else {
        Write-Host " Error" -ForegroundColor Red
        Write-Host $output
    }
    
    # Clean up temp file
    Remove-Item $sqlFile -Force -ErrorAction SilentlyContinue
}

Write-Host "`nFinished adding columns to order_items table!" -ForegroundColor Green 