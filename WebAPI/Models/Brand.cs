using System.ComponentModel.DataAnnotations;

namespace EcommerceAPI.Models
{
    public class Brand
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [MaxLength(500)]
        public string? LogoUrl { get; set; }
        
        public string? Description { get; set; }
        public bool IsFeatured { get; set; } = false;
        public int DisplayOrder { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
} 