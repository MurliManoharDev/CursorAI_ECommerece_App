using System.ComponentModel.DataAnnotations;

namespace EcommerceAPI.Models
{
    public class PromotionalItem
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(255)]
        public string Title { get; set; } = string.Empty;
        
        [MaxLength(255)]
        public string? Subtitle { get; set; }
        
        public string? Description { get; set; }
        
        [MaxLength(500)]
        public string? ImageUrl { get; set; }
        
        [MaxLength(50)]
        public string? ButtonText { get; set; }
        
        [MaxLength(500)]
        public string? ButtonLink { get; set; }
        
        public int? CategoryId { get; set; }
        
        [MaxLength(50)]
        public string? Position { get; set; } // 'banner', 'sidebar', 'popup', etc.
        
        public bool IsActive { get; set; } = true;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int DisplayOrder { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public Category? Category { get; set; }
    }
} 