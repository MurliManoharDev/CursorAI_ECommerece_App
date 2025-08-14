# PowerShell script to make user_id nullable in orders table

Write-Host "Making user_id nullable in orders table for guest checkout..." -ForegroundColor Green

# SQLite doesn't support ALTER COLUMN, so we need to recreate the table
$sqlContent = @"
-- Save existing order data
CREATE TABLE orders_backup AS SELECT * FROM orders;

-- Drop the existing orders table
DROP TABLE orders;

-- Recreate orders table with user_id as nullable
CREATE TABLE orders (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    order_number TEXT,
    user_id INTEGER,  -- Now nullable for guest checkout
    first_name TEXT,
    last_name TEXT,
    company_name TEXT,
    email TEXT,
    phone_number TEXT,
    street_address TEXT,
    apartment_suite TEXT,
    city TEXT,
    state TEXT,
    country TEXT,
    zip_code TEXT,
    subtotal REAL DEFAULT 0,
    shipping_cost REAL DEFAULT 0,
    tax REAL DEFAULT 0,
    total REAL DEFAULT 0,
    total_amount REAL DEFAULT 0,
    order_notes TEXT,
    payment_method TEXT,
    payment_intent_id TEXT,
    stripe_payment_id TEXT,
    payment_status INTEGER DEFAULT 0,
    status INTEGER DEFAULT 0,
    created_at TEXT,
    updated_at TEXT,
    shipped_at TEXT,
    delivered_at TEXT,
    FOREIGN KEY (user_id) REFERENCES users(id)
);

-- Restore data
INSERT INTO orders SELECT * FROM orders_backup;

-- Drop backup table
DROP TABLE orders_backup;
"@

$sqlFile = "temp_fix_user_id_nullable.sql"

# Write SQL to file
$sqlContent | Out-File -FilePath $sqlFile -Encoding UTF8

# Execute SQL file
Write-Host "Executing SQL to make user_id nullable..."
& dotnet run --project ../SeedDatabase/ExecuteSqlFile.csproj $sqlFile ../WebAPI/ecommerce.db

# Check if successful
if ($LASTEXITCODE -eq 0) {
    Write-Host "Successfully made user_id nullable!" -ForegroundColor Green
    Write-Host "Guest checkout (without login) is now fully supported." -ForegroundColor Green
} else {
    Write-Host "There was an error fixing the column." -ForegroundColor Red
}

# Clean up temp file
Remove-Item $sqlFile -Force -ErrorAction SilentlyContinue

Write-Host "Done!" -ForegroundColor Green 