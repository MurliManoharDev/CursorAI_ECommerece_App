using System.ComponentModel.DataAnnotations;

namespace EcommerceAPI.Models
{
    public class Category
    {
        public int Id { get; set; }
        
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
        
        public int? ParentId { get; set; }
        public int DisplayOrder { get; set; } = 0;
        public bool IsActive { get; set; } = true;
        public bool ShowInMenu { get; set; } = true;
        public bool ShowInSale { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public Category? Parent { get; set; }
        public ICollection<Category> Children { get; set; } = new List<Category>();
        public ICollection<Subcategory> Subcategories { get; set; } = new List<Subcategory>();
        public ICollection<Product> Products { get; set; } = new List<Product>();
        public ICollection<PromotionalItem> PromotionalItems { get; set; } = new List<PromotionalItem>();
    }
} 