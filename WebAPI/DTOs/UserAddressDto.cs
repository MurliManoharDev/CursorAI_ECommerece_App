using System;
using System.ComponentModel.DataAnnotations;

namespace EcommerceAPI.DTOs
{
    public class UserAddressDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string AddressType { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? CompanyName { get; set; }
        public string StreetAddress { get; set; } = string.Empty;
        public string? ApartmentSuite { get; set; }
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string ZipCode { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public bool IsDefault { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
    
    public class CreateAddressDto
    {
        [Required]
        public string AddressType { get; set; } = "shipping";
        
        [Required]
        public string FirstName { get; set; } = string.Empty;
        
        [Required]
        public string LastName { get; set; } = string.Empty;
        
        public string? CompanyName { get; set; }
        
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
        
        [Required]
        public string PhoneNumber { get; set; } = string.Empty;
        
        public bool IsDefault { get; set; }
    }
} 