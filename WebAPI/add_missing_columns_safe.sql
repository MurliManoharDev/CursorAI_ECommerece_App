-- Safely add missing columns to orders table
-- This script checks if columns exist before adding them

-- First, let's see what columns currently exist
PRAGMA table_info(orders);

-- Add missing columns one by one
-- Note: SQLite doesn't support IF NOT EXISTS for ALTER TABLE, so we'll handle errors gracefully

-- Address fields
ALTER TABLE orders ADD COLUMN city TEXT;
ALTER TABLE orders ADD COLUMN state TEXT;
ALTER TABLE orders ADD COLUMN country TEXT;
ALTER TABLE orders ADD COLUMN zip_code TEXT;

-- Billing information fields
ALTER TABLE orders ADD COLUMN first_name TEXT;
ALTER TABLE orders ADD COLUMN last_name TEXT;
ALTER TABLE orders ADD COLUMN company_name TEXT;
ALTER TABLE orders ADD COLUMN email TEXT;
ALTER TABLE orders ADD COLUMN phone_number TEXT;
ALTER TABLE orders ADD COLUMN street_address TEXT;

-- Order details
ALTER TABLE orders ADD COLUMN order_notes TEXT;

-- Payment information
ALTER TABLE orders ADD COLUMN payment_method TEXT;
ALTER TABLE orders ADD COLUMN payment_intent_id TEXT;
ALTER TABLE orders ADD COLUMN stripe_payment_id TEXT;
ALTER TABLE orders ADD COLUMN payment_status INTEGER DEFAULT 0;

-- Order status
ALTER TABLE orders ADD COLUMN status INTEGER DEFAULT 0;

-- Date fields
ALTER TABLE orders ADD COLUMN created_at TEXT;
ALTER TABLE orders ADD COLUMN updated_at TEXT;
ALTER TABLE orders ADD COLUMN shipped_at TEXT;
ALTER TABLE orders ADD COLUMN delivered_at TEXT; 