using Microsoft.EntityFrameworkCore;
using EcommerceAPI.Models;

namespace EcommerceAPI.Data
{
    public class EcommerceDbContext : DbContext
    {
        public EcommerceDbContext(DbContextOptions<EcommerceDbContext> options)
            : base(options)
        {
        }

        // DbSets
        public DbSet<User> Users { get; set; }
        public DbSet<UserAddress> UserAddresses { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Subcategory> Subcategories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductVariant> ProductVariants { get; set; }
        public DbSet<ProductTag> ProductTags { get; set; }
        public DbSet<ProductReview> ProductReviews { get; set; }
        public DbSet<PromotionalItem> PromotionalItems { get; set; }
        public DbSet<SliderItem> SliderItems { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<WishlistItem> WishlistItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<OrderStatusHistory> OrderStatusHistory { get; set; }
        public DbSet<RecentlyViewed> RecentlyViewed { get; set; }
        public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }
        public DbSet<FrequentlyBoughtTogether> FrequentlyBoughtTogether { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure table names (matching the existing database)
            modelBuilder.Entity<CartItem>().ToTable("cart_items");
            modelBuilder.Entity<WishlistItem>().ToTable("wishlist_items");
            modelBuilder.Entity<RecentlyViewed>().ToTable("recently_viewed");
            modelBuilder.Entity<SliderItem>().ToTable("slider_items");

            // User entity configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.Email).IsRequired();
                entity.Property(e => e.PasswordHash).IsRequired();
            });

            // UserAddress entity configuration
            modelBuilder.Entity<UserAddress>(entity =>
            {
                entity.HasOne(e => e.User)
                    .WithMany(u => u.Addresses)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Currency entity configuration
            modelBuilder.Entity<Currency>(entity =>
            {
                entity.HasIndex(e => e.Code).IsUnique();
                entity.Property(e => e.ExchangeRate).HasPrecision(10, 4);
            });

            // Language entity configuration
            modelBuilder.Entity<Language>(entity =>
            {
                entity.HasIndex(e => e.Code).IsUnique();
            });

            // Brand entity configuration
            modelBuilder.Entity<Brand>(entity =>
            {
                entity.HasIndex(e => e.Name).IsUnique();
            });

            // Category entity configuration
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasIndex(e => e.Slug).IsUnique();
                entity.HasOne(e => e.Parent)
                    .WithMany(p => p.Children)
                    .HasForeignKey(e => e.ParentId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Subcategory entity configuration
            modelBuilder.Entity<Subcategory>(entity =>
            {
                entity.HasIndex(e => new { e.CategoryId, e.Slug }).IsUnique();
                entity.HasOne(e => e.Category)
                    .WithMany(c => c.Subcategories)
                    .HasForeignKey(e => e.CategoryId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Product entity configuration
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasIndex(e => e.Slug).IsUnique();
                entity.HasIndex(e => e.Sku).IsUnique();
                entity.Property(e => e.Price).HasPrecision(10, 2);
                entity.Property(e => e.OldPrice).HasPrecision(10, 2);
                entity.Property(e => e.Cost).HasPrecision(10, 2);
                entity.Property(e => e.ShippingCost).HasPrecision(10, 2);
                entity.Property(e => e.Weight).HasPrecision(10, 3);
                entity.Property(e => e.DimensionsLength).HasPrecision(10, 2);
                entity.Property(e => e.DimensionsWidth).HasPrecision(10, 2);
                entity.Property(e => e.DimensionsHeight).HasPrecision(10, 2);
                
                entity.HasOne(e => e.Brand)
                    .WithMany(b => b.Products)
                    .HasForeignKey(e => e.BrandId);
                    
                entity.HasOne(e => e.Category)
                    .WithMany(c => c.Products)
                    .HasForeignKey(e => e.CategoryId);
                    
                entity.HasOne(e => e.Subcategory)
                    .WithMany(s => s.Products)
                    .HasForeignKey(e => e.SubcategoryId);
            });

            // ProductVariant entity configuration
            modelBuilder.Entity<ProductVariant>(entity =>
            {
                entity.HasIndex(e => e.Sku).IsUnique();
                entity.Property(e => e.PriceAdjustment).HasPrecision(10, 2);
                entity.HasOne(e => e.Product)
                    .WithMany(p => p.Variants)
                    .HasForeignKey(e => e.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ProductTag entity configuration
            modelBuilder.Entity<ProductTag>(entity =>
            {
                entity.HasIndex(e => new { e.ProductId, e.TagName }).IsUnique();
                entity.HasOne(e => e.Product)
                    .WithMany(p => p.Tags)
                    .HasForeignKey(e => e.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ProductReview entity configuration
            modelBuilder.Entity<ProductReview>(entity =>
            {
                entity.HasOne(e => e.Product)
                    .WithMany(p => p.Reviews)
                    .HasForeignKey(e => e.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);
                    
                entity.HasOne(e => e.User)
                    .WithMany(u => u.Reviews)
                    .HasForeignKey(e => e.UserId);
            });

            // CartItem entity configuration
            modelBuilder.Entity<CartItem>(entity =>
            {
                entity.HasIndex(e => new { e.UserId, e.ProductId, e.VariantId }).IsUnique();
                entity.HasOne(e => e.User)
                    .WithMany(u => u.CartItems)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
                    
                entity.HasOne(e => e.Product)
                    .WithMany(p => p.CartItems)
                    .HasForeignKey(e => e.ProductId);
                    
                entity.HasOne(e => e.Variant)
                    .WithMany(v => v.CartItems)
                    .HasForeignKey(e => e.VariantId);
            });

            // WishlistItem entity configuration
            modelBuilder.Entity<WishlistItem>(entity =>
            {
                entity.HasIndex(e => new { e.UserId, e.ProductId }).IsUnique();
                entity.HasOne(e => e.User)
                    .WithMany(u => u.WishlistItems)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
                    
                entity.HasOne(e => e.Product)
                    .WithMany(p => p.WishlistItems)
                    .HasForeignKey(e => e.ProductId);
            });

            // Order entity configuration
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasIndex(e => e.OrderNumber).IsUnique();
                entity.Property(e => e.Subtotal).HasPrecision(10, 2);
                entity.Property(e => e.Tax).HasPrecision(10, 2);
                entity.Property(e => e.ShippingCost).HasPrecision(10, 2);
                entity.Property(e => e.Total).HasPrecision(10, 2).HasColumnName("total_amount");
                
                entity.HasOne(e => e.User)
                    .WithMany(u => u.Orders)
                    .HasForeignKey(e => e.UserId);
            });

            // OrderItem entity configuration
            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.Property(e => e.Price).HasPrecision(10, 2).HasColumnName("unit_price");
                entity.Property(e => e.Total).HasPrecision(10, 2);
                entity.Property(e => e.ShippingCost).HasPrecision(10, 2);
                
                entity.HasOne(e => e.Order)
                    .WithMany(o => o.OrderItems)
                    .HasForeignKey(e => e.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);
                    
                // Make Product relationship optional
                entity.HasOne(e => e.Product)
                    .WithMany(p => p.OrderItems)
                    .HasForeignKey(e => e.ProductId)
                    .IsRequired(false);
            });

            // OrderStatusHistory entity configuration
            modelBuilder.Entity<OrderStatusHistory>(entity =>
            {
                entity.HasOne(e => e.Order)
                    .WithMany(o => o.StatusHistory)
                    .HasForeignKey(e => e.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // RecentlyViewed entity configuration
            modelBuilder.Entity<RecentlyViewed>(entity =>
            {
                entity.HasIndex(e => new { e.UserId, e.ProductId }).IsUnique();
                entity.HasOne(e => e.User)
                    .WithMany(u => u.RecentlyViewedProducts)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
                    
                entity.HasOne(e => e.Product)
                    .WithMany(p => p.RecentlyViewedByUsers)
                    .HasForeignKey(e => e.ProductId);
            });

            // PromotionalItem entity configuration
            modelBuilder.Entity<PromotionalItem>(entity =>
            {
                entity.HasOne(e => e.Category)
                    .WithMany(c => c.PromotionalItems)
                    .HasForeignKey(e => e.CategoryId);
            });

            // PasswordResetToken entity configuration
            modelBuilder.Entity<PasswordResetToken>(entity =>
            {
                entity.ToTable("password_reset_tokens");
                entity.HasIndex(e => e.Token).IsUnique();
                entity.HasIndex(e => new { e.UserId, e.ExpiresAt });
                
                entity.HasOne(e => e.User)
                    .WithMany(u => u.PasswordResetTokens)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // FrequentlyBoughtTogether entity configuration
            modelBuilder.Entity<FrequentlyBoughtTogether>(entity =>
            {
                entity.ToTable("frequently_bought_together");
                entity.HasIndex(e => new { e.ProductId, e.RelatedProductId }).IsUnique();
                
                entity.HasOne(e => e.Product)
                    .WithMany()
                    .HasForeignKey(e => e.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);
                    
                entity.HasOne(e => e.RelatedProduct)
                    .WithMany()
                    .HasForeignKey(e => e.RelatedProductId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure column names to match existing database
            ConfigureColumnNames(modelBuilder);
        }

        private void ConfigureColumnNames(ModelBuilder modelBuilder)
        {
            // Map properties to snake_case column names
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                // Convert table names to snake_case
                var tableName = entity.GetTableName();
                if (tableName != null)
                {
                    entity.SetTableName(ToSnakeCase(tableName));
                }

                // Convert column names to snake_case
                foreach (var property in entity.GetProperties())
                {
                    // Skip if column name was explicitly set (e.g., Total -> total_amount)
                    var currentColumnName = property.GetColumnName();
                    var defaultColumnName = property.Name;
                    
                    // Only apply snake_case if no explicit mapping was defined
                    if (currentColumnName == defaultColumnName)
                    {
                        property.SetColumnName(ToSnakeCase(property.Name));
                    }
                }

                // Convert foreign key names to snake_case
                foreach (var key in entity.GetForeignKeys())
                {
                    foreach (var property in key.Properties)
                    {
                        if (!property.GetColumnName().EndsWith("_id"))
                        {
                            property.SetColumnName(ToSnakeCase(property.Name));
                        }
                    }
                }
            }
        }

        private string ToSnakeCase(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            var result = string.Empty;
            for (int i = 0; i < input.Length; i++)
            {
                if (char.IsUpper(input[i]) && i > 0)
                {
                    result += "_";
                }
                result += char.ToLower(input[i]);
            }
            return result;
        }
    }
} 