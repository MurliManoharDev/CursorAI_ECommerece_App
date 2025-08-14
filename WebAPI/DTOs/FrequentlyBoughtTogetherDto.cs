namespace EcommerceAPI.DTOs
{
    public class FrequentlyBoughtTogetherDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int RelatedProductId { get; set; }
        public ProductDto? RelatedProduct { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateFrequentlyBoughtTogetherDto
    {
        public int ProductId { get; set; }
        public int[] RelatedProductIds { get; set; } = Array.Empty<int>();
    }

    public class FrequentlyBoughtTogetherProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal? OldPrice { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public bool Selected { get; set; } = true;
    }

    public class FrequentlyBoughtTogetherResponseDto
    {
        public List<FrequentlyBoughtTogetherProductDto> Products { get; set; } = new();
        public decimal TotalPrice { get; set; }
        public decimal TotalOldPrice { get; set; }
        public decimal TotalSavings { get; set; }
    }
} 