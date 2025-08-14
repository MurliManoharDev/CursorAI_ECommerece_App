using System.ComponentModel.DataAnnotations;

namespace EcommerceAPI.Models
{
    public class OrderItem
    {
        [Key]
        public int Id { get; set; }
        
        public int OrderId { get; set; }
        public Order Order { get; set; } = null!;
        
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
        
        [Required]
        public string ProductName { get; set; } = string.Empty;
        
        public string? ProductImage { get; set; }
        
        public int Quantity { get; set; }
        
        public decimal Price { get; set; }
        
        public decimal Total { get; set; }
        
        public string? ShippingType { get; set; }
        
        public decimal ShippingCost { get; set; }
    }
} 