using System.ComponentModel.DataAnnotations;

namespace EcommerceAPI.Models
{
    public class Product
    {
        public int Id { get; set; }
        
        [MaxLength(100)]
        public string? Sku { get; set; }
        
        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;
        
        [MaxLength(255)]
        public string? Subtitle { get; set; }
        
        [Required]
        [MaxLength(255)]
        public string Slug { get; set; } = string.Empty;
        
        public string? Description { get; set; }
        
        [Required]
        [MaxLength(500)]
        public string ImageUrl { get; set; } = string.Empty;
        
        [Required]
        public decimal Price { get; set; }
        
        public decimal? OldPrice { get; set; }
        public decimal Cost { get; set; } = 0;
        public int? BrandId { get; set; }
        public int CategoryId { get; set; }
        public int? SubcategoryId { get; set; }
        public int StockQuantity { get; set; } = 0;
        public int LowStockThreshold { get; set; } = 10;
        public decimal? Weight { get; set; } // in kg
        public decimal? DimensionsLength { get; set; } // in cm
        public decimal? DimensionsWidth { get; set; }
        public decimal? DimensionsHeight { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsFeatured { get; set; } = false;
        public bool IsNew { get; set; } = false;
        public bool IsOnSale { get; set; } = false;
        public bool FreeShipping { get; set; } = false;
        public decimal ShippingCost { get; set; } = 0;
        public bool FreeGift { get; set; } = false;
        public bool ContactForPrice { get; set; } = false;
        public int ViewsCount { get; set; } = 0;
        public int SalesCount { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public Brand? Brand { get; set; }
        public Category Category { get; set; } = null!;
        public Subcategory? Subcategory { get; set; }
        public ICollection<ProductVariant> Variants { get; set; } = new List<ProductVariant>();
        public ICollection<ProductTag> Tags { get; set; } = new List<ProductTag>();
        public ICollection<ProductReview> Reviews { get; set; } = new List<ProductReview>();
        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
        public ICollection<WishlistItem> WishlistItems { get; set; } = new List<WishlistItem>();
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public ICollection<RecentlyViewed> RecentlyViewedByUsers { get; set; } = new List<RecentlyViewed>();
    }
} 