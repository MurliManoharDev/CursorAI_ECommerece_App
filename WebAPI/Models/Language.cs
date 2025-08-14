using System.ComponentModel.DataAnnotations;

namespace EcommerceAPI.Models
{
    public class Language
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(2)]
        public string Code { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;
        
        [MaxLength(100)]
        public string? FlagIcon { get; set; }
        
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public ICollection<User> Users { get; set; } = new List<User>();
    }
} 