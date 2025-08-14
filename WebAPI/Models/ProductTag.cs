using System.ComponentModel.DataAnnotations;

namespace EcommerceAPI.Models
{
    public class ProductTag
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string TagName { get; set; } = string.Empty;
        
        [MaxLength(20)]
        public string? TagType { get; set; } // 'shipping', 'promotion', 'feature', etc.
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public Product Product { get; set; } = null!;
    }
} 