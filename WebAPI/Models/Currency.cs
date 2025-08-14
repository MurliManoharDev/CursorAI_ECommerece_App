using System.ComponentModel.DataAnnotations;

namespace EcommerceAPI.Models
{
    public class Currency
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(3)]
        public string Code { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(5)]
        public string Symbol { get; set; } = string.Empty;
        
        public decimal ExchangeRate { get; set; } = 1.0000m;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public ICollection<User> Users { get; set; } = new List<User>();
    }
} 