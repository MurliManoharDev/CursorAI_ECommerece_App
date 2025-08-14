using System.ComponentModel.DataAnnotations;

namespace EcommerceAPI.Models
{
    public class ProductVariant
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        
        [MaxLength(100)]
        public string? VariantName { get; set; }
        
        [MaxLength(50)]
        public string? Color { get; set; }
        
        [MaxLength(50)]
        public string? Size { get; set; }
        
        [MaxLength(500)]
        public string? ImageUrl { get; set; }
        
        public decimal PriceAdjustment { get; set; } = 0;
        public int StockQuantity { get; set; } = 0;
        
        [MaxLength(100)]
        public string? Sku { get; set; }
        
        public int DisplayOrder { get; set; } = 0;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public Product Product { get; set; } = null!;
        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
} 