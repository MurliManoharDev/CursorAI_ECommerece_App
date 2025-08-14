-- Add apartment_suite column to orders table if it doesn't exist
ALTER TABLE orders ADD COLUMN apartment_suite TEXT; 