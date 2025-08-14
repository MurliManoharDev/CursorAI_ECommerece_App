using Microsoft.EntityFrameworkCore;
using EcommerceAPI.Data;
using EcommerceAPI.Models;

namespace EcommerceAPI.Data
{
    public static class DatabaseInitializer
    {
        public static void Initialize(EcommerceDbContext context)
        {
            CreateTablesIfNotExists(context);
            SeedData(context);
        }

        private static void CreateTablesIfNotExists(EcommerceDbContext context)
        {
            // Create password_reset_tokens table if not exists
            context.Database.ExecuteSqlRaw(@"
                CREATE TABLE IF NOT EXISTS password_reset_tokens (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    user_id INTEGER NOT NULL,
                    token TEXT NOT NULL UNIQUE,
                    expires_at DATETIME NOT NULL,
                    used_at DATETIME,
                    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
                    FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE
                );
            ");

            context.Database.ExecuteSqlRaw(@"
                CREATE INDEX IF NOT EXISTS idx_password_reset_tokens_user_expires 
                ON password_reset_tokens(user_id, expires_at);
            ");

            // Create frequently_bought_together table if not exists
            context.Database.ExecuteSqlRaw(@"
                CREATE TABLE IF NOT EXISTS frequently_bought_together (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    product_id INTEGER NOT NULL,
                    related_product_id INTEGER NOT NULL,
                    frequency INTEGER DEFAULT 1,
                    FOREIGN KEY (product_id) REFERENCES products(id) ON DELETE CASCADE,
                    FOREIGN KEY (related_product_id) REFERENCES products(id) ON DELETE CASCADE,
                    UNIQUE(product_id, related_product_id)
                );
            ");

            context.Database.ExecuteSqlRaw(@"
                CREATE INDEX IF NOT EXISTS idx_frequently_bought_product_id 
                ON frequently_bought_together(product_id);
            ");
            
            // Create order_status_history table
            context.Database.ExecuteSqlRaw(@"
                CREATE TABLE IF NOT EXISTS order_status_history (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    order_id INTEGER NOT NULL,
                    status TEXT NOT NULL,
                    notes TEXT,
                    user_id INTEGER,
                    created_at DATETIME NOT NULL,
                    FOREIGN KEY (order_id) REFERENCES orders(id) ON DELETE CASCADE,
                    FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE SET NULL
                );
            ");

            context.Database.ExecuteSqlRaw(@"
                CREATE INDEX IF NOT EXISTS idx_order_status_history_order_id 
                ON order_status_history(order_id);
            ");
            
            // Create user_addresses table if not exists
            context.Database.ExecuteSqlRaw(@"
                CREATE TABLE IF NOT EXISTS user_addresses (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    user_id INTEGER NOT NULL,
                    address_type TEXT NOT NULL DEFAULT 'shipping',
                    first_name TEXT NOT NULL,
                    last_name TEXT NOT NULL,
                    company_name TEXT,
                    street_address TEXT NOT NULL,
                    apartment_suite TEXT,
                    city TEXT NOT NULL,
                    state TEXT NOT NULL,
                    country TEXT NOT NULL,
                    zip_code TEXT NOT NULL,
                    phone_number TEXT NOT NULL,
                    is_default BOOLEAN DEFAULT 0,
                    created_at DATETIME NOT NULL,
                    updated_at DATETIME NOT NULL,
                    FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE
                );
            ");

            context.Database.ExecuteSqlRaw(@"
                CREATE INDEX IF NOT EXISTS idx_user_addresses_user_id 
                ON user_addresses(user_id);
            ");
        }

        private static void SeedData(EcommerceDbContext context)
        {
            // Add sample data if table is empty
            var hasData = context.FrequentlyBoughtTogether.Any();
            if (!hasData)
            {
                SeedFrequentlyBoughtTogether(context);
            }
        }

        private static void SeedFrequentlyBoughtTogether(EcommerceDbContext context)
        {
            // Check if products exist before seeding
            var productIds = context.Products.Select(p => p.Id).OrderBy(p => p).ToList();
            if (productIds.Count < 2) return;

            var frequentlyBoughtItems = new List<FrequentlyBoughtTogether>();

            // Add relationships for multiple products
            var random = new Random();
            foreach (var productId in productIds.Take(30)) // Seed for first 30 products
            {
                // Skip if already has relationships
                if (context.FrequentlyBoughtTogether.Any(f => f.ProductId == productId))
                    continue;

                // Add 2-3 related products for each product
                var relatedCount = random.Next(2, 4);
                var availableProducts = productIds.Where(p => p != productId).ToList();
                
                for (int i = 0; i < relatedCount && i < availableProducts.Count; i++)
                {
                    var relatedIndex = random.Next(availableProducts.Count);
                    var relatedProductId = availableProducts[relatedIndex];
                    availableProducts.RemoveAt(relatedIndex);

                    frequentlyBoughtItems.Add(new FrequentlyBoughtTogether
                    {
                        ProductId = productId,
                        RelatedProductId = relatedProductId,
                        DisplayOrder = i,
                        IsActive = true
                    });
                }
            }

            if (frequentlyBoughtItems.Any())
            {
                context.FrequentlyBoughtTogether.AddRange(frequentlyBoughtItems);
                context.SaveChanges();
            }
        }
    }
} 