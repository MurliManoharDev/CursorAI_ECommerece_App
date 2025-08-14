# PowerShell script to ensure total_amount has a default value

Write-Host "Setting default value for total_amount column..." -ForegroundColor Green

# Create SQL file
$sqlContent = @"
-- SQLite doesn't support ALTER COLUMN to add default, so we need to recreate the table
-- First, let's create a temporary table with the correct schema
CREATE TABLE orders_temp AS SELECT * FROM orders;

-- Drop the original table
DROP TABLE orders;

-- Recreate orders table with total_amount having a default value
CREATE TABLE orders (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    order_number TEXT,
    user_id INTEGER NOT NULL,
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
    total_amount REAL DEFAULT 0,  -- Now with default value
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

-- Copy data back
INSERT INTO orders SELECT * FROM orders_temp;

-- Drop temporary table
DROP TABLE orders_temp;
"@

$sqlFile = "temp_fix_total_amount_default.sql"

# Write SQL to file
$sqlContent | Out-File -FilePath $sqlFile -Encoding UTF8

# Execute SQL file
Write-Host "Executing SQL to fix total_amount default..."
& dotnet run --project ../SeedDatabase/ExecuteSqlFile.csproj $sqlFile ../WebAPI/ecommerce.db

# Check if successful
if ($LASTEXITCODE -eq 0) {
    Write-Host "Successfully added default value to total_amount column!" -ForegroundColor Green
} else {
    Write-Host "There was an error fixing the column." -ForegroundColor Yellow
}

# Clean up temp file
Remove-Item $sqlFile -Force -ErrorAction SilentlyContinue

Write-Host "Done!" -ForegroundColor Green 