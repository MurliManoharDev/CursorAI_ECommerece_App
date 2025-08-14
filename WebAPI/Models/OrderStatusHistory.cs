using System;
using System.ComponentModel.DataAnnotations;

namespace EcommerceAPI.Models
{
    public class OrderStatusHistory
    {
        [Key]
        public int Id { get; set; }
        
        public int OrderId { get; set; }
        public Order Order { get; set; } = null!;
        
        [Required]
        public string Status { get; set; } = string.Empty;
        
        public string? Notes { get; set; }
        
        public int? UserId { get; set; }
        public User? User { get; set; }
        
        public DateTime CreatedAt { get; set; }
    }
} 