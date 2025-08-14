using System.ComponentModel.DataAnnotations;

namespace EcommerceAPI.Models
{
    public class SliderItem
    {
        public int Id { get; set; }
        
        [MaxLength(255)]
        public string? Title { get; set; }
        
        [MaxLength(255)]
        public string? Subtitle { get; set; }
        
        public string? Description { get; set; }
        
        [Required]
        [MaxLength(500)]
        public string ImageUrl { get; set; } = string.Empty;
        
        [MaxLength(50)]
        public string? ButtonText { get; set; }
        
        [MaxLength(500)]
        public string? ButtonLink { get; set; }
        
        [MaxLength(50)]
        public string? PriceText { get; set; }
        
        public string? Features { get; set; } // JSON array of features
        
        [MaxLength(20)]
        public string Position { get; set; } = "main"; // main, side, bottom
        
        public bool IsActive { get; set; } = true;
        public int DisplayOrder { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
} 