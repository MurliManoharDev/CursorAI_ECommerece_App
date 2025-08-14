using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EcommerceAPI.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string OrderNumber { get; set; } = string.Empty;
        
        public int? UserId { get; set; }
        public User? User { get; set; }
        
        // Billing Information
        [Required]
        public string FirstName { get; set; } = string.Empty;
        
        [Required]
        public string LastName { get; set; } = string.Empty;
        
        public string? CompanyName { get; set; }
        
        [Required]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        public string PhoneNumber { get; set; } = string.Empty;
        
        // Shipping Address
        [Required]
        public string StreetAddress { get; set; } = string.Empty;
        
        public string? ApartmentSuite { get; set; }
        
        [Required]
        public string City { get; set; } = string.Empty;
        
        [Required]
        public string State { get; set; } = string.Empty;
        
        [Required]
        public string Country { get; set; } = string.Empty;
        
        [Required]
        public string ZipCode { get; set; } = string.Empty;
        
        // Order Details
        public decimal Subtotal { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal Tax { get; set; }
        public decimal Total { get; set; }
        
        public string? OrderNotes { get; set; }
        
        // Payment Information
        [Required]
        public string PaymentMethod { get; set; } = string.Empty;
        public string? PaymentIntentId { get; set; }
        public string? StripePaymentId { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        
        // Order Status
        public OrderStatus Status { get; set; }
        
        // Dates
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? ShippedAt { get; set; }
        public DateTime? DeliveredAt { get; set; }
        
        // Navigation Properties
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public ICollection<OrderStatusHistory> StatusHistory { get; set; } = new List<OrderStatusHistory>();
    }
    
    public enum OrderStatus
    {
        Pending,
        Processing,
        Confirmed,
        Shipped,
        Delivered,
        Cancelled,
        Refunded
    }
    
    public enum PaymentStatus
    {
        Pending,
        Processing,
        Succeeded,
        Failed,
        Cancelled,
        Refunded
    }
} 