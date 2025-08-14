using System.ComponentModel.DataAnnotations;

namespace EcommerceAPI.Models
{
    public class Subcategory
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(100)]
        public string Slug { get; set; } = string.Empty;
        
        public string? Description { get; set; }
        
        [MaxLength(50)]
        public string? IconClass { get; set; }
        
        [MaxLength(500)]
        public string? ImageUrl { get; set; }
        
        public int ItemCount { get; set; } = 0;
        public int DisplayOrder { get; set; } = 0;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public Category Category { get; set; } = null!;
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
} 