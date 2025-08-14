namespace EcommerceAPI.DTOs
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string? Sku { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Subtitle { get; set; }
        public string Slug { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal? OldPrice { get; set; }
        public int? BrandId { get; set; }
        public string? BrandName { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public int? SubcategoryId { get; set; }
        public string? SubcategoryName { get; set; }
        public int StockQuantity { get; set; }
        public bool IsActive { get; set; }
        public bool IsFeatured { get; set; }
        public bool IsNew { get; set; }
        public bool IsOnSale { get; set; }
        public bool FreeShipping { get; set; }
        public decimal ShippingCost { get; set; }
        public bool FreeGift { get; set; }
        public bool ContactForPrice { get; set; }
        public int ViewsCount { get; set; }
        public int SalesCount { get; set; }
        public double? AverageRating { get; set; }
        public int ReviewCount { get; set; }
        public List<ProductVariantDto> Variants { get; set; } = new();
        public List<ProductTagDto> Tags { get; set; } = new();
    }

    public class ProductListDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Subtitle { get; set; }
        public string Slug { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal? OldPrice { get; set; }
        public string? BrandName { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public bool IsOnSale { get; set; }
        public bool FreeShipping { get; set; }
        public double? AverageRating { get; set; }
        public int ReviewCount { get; set; }
    }

    public class ProductCreateDto
    {
        public string? Sku { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Subtitle { get; set; }
        public string Slug { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal? OldPrice { get; set; }
        public decimal Cost { get; set; } = 0;
        public int? BrandId { get; set; }
        public int CategoryId { get; set; }
        public int? SubcategoryId { get; set; }
        public int StockQuantity { get; set; } = 0;
        public int LowStockThreshold { get; set; } = 10;
        public decimal? Weight { get; set; }
        public decimal? DimensionsLength { get; set; }
        public decimal? DimensionsWidth { get; set; }
        public decimal? DimensionsHeight { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsFeatured { get; set; } = false;
        public bool IsNew { get; set; } = false;
        public bool IsOnSale { get; set; } = false;
        public bool FreeShipping { get; set; } = false;
        public decimal ShippingCost { get; set; } = 0;
        public bool FreeGift { get; set; } = false;
        public bool ContactForPrice { get; set; } = false;
    }

    public class ProductUpdateDto : ProductCreateDto
    {
        public int Id { get; set; }
    }

    public class ProductVariantDto
    {
        public int Id { get; set; }
        public string? VariantName { get; set; }
        public string? Color { get; set; }
        public string? Size { get; set; }
        public string? ImageUrl { get; set; }
        public decimal PriceAdjustment { get; set; }
        public int StockQuantity { get; set; }
        public string? Sku { get; set; }
        public bool IsActive { get; set; }
    }

    public class ProductTagDto
    {
        public int Id { get; set; }
        public string TagName { get; set; } = string.Empty;
        public string? TagType { get; set; }
    }
} 