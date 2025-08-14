# PowerShell script to recreate order_items table with correct columns

Write-Host "Recreating order_items table with correct columns..." -ForegroundColor Green

# Create SQL file
$sqlContent = @"
-- Drop existing order_items table
DROP TABLE IF EXISTS order_items;

-- Recreate order_items table with correct columns
CREATE TABLE order_items (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    order_id INTEGER NOT NULL,
    product_id INTEGER NOT NULL,
    product_name TEXT NOT NULL,
    product_image TEXT,
    quantity INTEGER NOT NULL DEFAULT 1,
    unit_price REAL NOT NULL,  -- Maps to Price property
    total REAL NOT NULL,        -- Maps to Total property
    shipping_type TEXT,
    shipping_cost REAL DEFAULT 0,
    product_variant_id INTEGER, -- Added for variant support
    FOREIGN KEY (order_id) REFERENCES orders(id) ON DELETE CASCADE,
    FOREIGN KEY (product_id) REFERENCES products(id)
);

-- Create index for better query performance
CREATE INDEX idx_order_items_order_id ON order_items(order_id);
CREATE INDEX idx_order_items_product_id ON order_items(product_id);
"@

$sqlFile = "temp_recreate_order_items.sql"

# Write SQL to file
$sqlContent | Out-File -FilePath $sqlFile -Encoding UTF8

# Execute SQL file
Write-Host "Executing SQL to recreate order_items table..."
& dotnet run --project ../SeedDatabase/ExecuteSqlFile.csproj $sqlFile ../WebAPI/ecommerce.db

# Check if successful
if ($LASTEXITCODE -eq 0) {
    Write-Host "Successfully recreated order_items table!" -ForegroundColor Green
    Write-Host "Table now has the following columns:" -ForegroundColor Cyan
    Write-Host "  - id (auto-increment)" -ForegroundColor Gray
    Write-Host "  - order_id (FK to orders)" -ForegroundColor Gray
    Write-Host "  - product_id (FK to products)" -ForegroundColor Gray
    Write-Host "  - product_name" -ForegroundColor Gray
    Write-Host "  - product_image" -ForegroundColor Gray
    Write-Host "  - quantity" -ForegroundColor Gray
    Write-Host "  - unit_price (maps to Price property)" -ForegroundColor Gray
    Write-Host "  - total (maps to Total property)" -ForegroundColor Gray
    Write-Host "  - shipping_type" -ForegroundColor Gray
    Write-Host "  - shipping_cost" -ForegroundColor Gray
    Write-Host "  - product_variant_id (nullable)" -ForegroundColor Gray
} else {
    Write-Host "Error recreating the table." -ForegroundColor Red
}

# Clean up temp file
Remove-Item $sqlFile -Force -ErrorAction SilentlyContinue

Write-Host "`nDone!" -ForegroundColor Green 