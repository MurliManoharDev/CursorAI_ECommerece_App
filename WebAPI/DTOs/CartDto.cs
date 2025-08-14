namespace EcommerceAPI.DTOs
{
    public class CartItemDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductImageUrl { get; set; } = string.Empty;
        public decimal ProductPrice { get; set; }
        public int? VariantId { get; set; }
        public string? VariantName { get; set; }
        public int Quantity { get; set; }
        public decimal ItemTotal { get; set; }
        public DateTime AddedAt { get; set; }
        
        // Additional properties needed for checkout
        public string Name => ProductName;
        public string Image => ProductImageUrl;
        public decimal Price => ProductPrice;
        public string? Shipping { get; set; }
        public decimal? ShippingCost { get; set; }
    }

    public class CartDto
    {
        public List<CartItemDto> Items { get; set; } = new();
        public decimal Subtotal { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal Tax { get; set; }
        public decimal Total { get; set; }
        public int ItemCount { get; set; }
    }

    public class AddToCartDto
    {
        public int ProductId { get; set; }
        public int? VariantId { get; set; }
        public int Quantity { get; set; } = 1;
    }

    public class UpdateCartItemDto
    {
        public int CartItemId { get; set; }
        public int Quantity { get; set; }
    }
} 