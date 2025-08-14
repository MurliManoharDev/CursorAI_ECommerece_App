$columns = @(
    "first_name TEXT",
    "last_name TEXT", 
    "company_name TEXT",
    "email TEXT",
    "phone_number TEXT",
    "street_address TEXT",
    "apartment_suite TEXT",
    "city TEXT",
    "state TEXT",
    "country TEXT",
    "zip_code TEXT",
    "order_notes TEXT",
    "payment_method TEXT",
    "payment_intent_id TEXT",
    "stripe_payment_id TEXT",
    "payment_status INTEGER DEFAULT 0",
    "status INTEGER DEFAULT 0",
    "created_at TEXT",
    "updated_at TEXT",
    "shipped_at TEXT",
    "delivered_at TEXT"
)

Write-Host "Adding missing columns to orders table..."

foreach ($column in $columns) {
    $columnName = $column.Split(" ")[0]
    $sqlFile = "temp_add_$columnName.sql"
    
    # Create SQL file for this column
    "ALTER TABLE orders ADD COLUMN $column;" | Out-File -FilePath $sqlFile -Encoding UTF8
    
    Write-Host "Adding column: $columnName..."
    
    # Execute SQL file, ignore errors
    & dotnet run --project ../SeedDatabase/ExecuteSqlFile.csproj $sqlFile ../WebAPI/ecommerce.db 2>$null
    
    # Clean up temp file
    Remove-Item $sqlFile -Force -ErrorAction SilentlyContinue
}

Write-Host "Finished adding columns!" 