using System.ComponentModel.DataAnnotations;

namespace EcommerceAPI.Models
{
    public class ProductReview
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int UserId { get; set; }
        
        [Range(1, 5)]
        public int Rating { get; set; }
        
        [MaxLength(255)]
        public string? Title { get; set; }
        
        public string? Comment { get; set; }
        public bool IsVerifiedPurchase { get; set; } = false;
        public int HelpfulCount { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public Product Product { get; set; } = null!;
        public User User { get; set; } = null!;
    }
} 