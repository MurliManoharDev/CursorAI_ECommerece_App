namespace EcommerceAPI.Models
{
    public class RecentlyViewed
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public DateTime ViewedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public User User { get; set; } = null!;
        public Product Product { get; set; } = null!;
    }
} 