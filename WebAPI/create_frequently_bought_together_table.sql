-- Create table for frequently bought together products
CREATE TABLE IF NOT EXISTS frequently_bought_together (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    product_id INTEGER NOT NULL,
    related_product_id INTEGER NOT NULL,
    display_order INTEGER DEFAULT 0,
    is_active BOOLEAN DEFAULT 1,
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (product_id) REFERENCES products(id) ON DELETE CASCADE,
    FOREIGN KEY (related_product_id) REFERENCES products(id) ON DELETE CASCADE,
    UNIQUE(product_id, related_product_id)
);

-- Create indexes for better performance
CREATE INDEX idx_frequently_bought_together_product_id ON frequently_bought_together(product_id);
CREATE INDEX idx_frequently_bought_together_related_product_id ON frequently_bought_together(related_product_id);

-- Insert some sample data
INSERT INTO frequently_bought_together (product_id, related_product_id, display_order, is_active) VALUES
-- For product 1 (assuming it's a phone)
(1, 2, 0, 1), -- Phone + Headphones
(1, 3, 1, 1), -- Phone + Watch
-- For product 2 (assuming it's headphones)
(2, 1, 0, 1), -- Headphones + Phone
(2, 4, 1, 1), -- Headphones + Case
-- For product 3 (assuming it's a watch)
(3, 1, 0, 1), -- Watch + Phone
(3, 5, 1, 1); -- Watch + Band 