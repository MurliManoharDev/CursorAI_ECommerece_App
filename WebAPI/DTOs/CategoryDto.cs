namespace EcommerceAPI.DTOs
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? IconClass { get; set; }
        public string? ImageUrl { get; set; }
        public int? ParentId { get; set; }
        public string? ParentName { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
        public bool ShowInMenu { get; set; }
        public bool ShowInSale { get; set; }
        public List<SubcategoryDto> Subcategories { get; set; } = new();
        public int ProductCount { get; set; }
    }

    public class CategoryListDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string? IconClass { get; set; }
        public string? ImageUrl { get; set; }
        public int ProductCount { get; set; }
        public List<SubcategoryListDto> Subcategories { get; set; } = new();
    }

    public class CategoryCreateDto
    {
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? IconClass { get; set; }
        public string? ImageUrl { get; set; }
        public int? ParentId { get; set; }
        public int DisplayOrder { get; set; } = 0;
        public bool IsActive { get; set; } = true;
        public bool ShowInMenu { get; set; } = true;
        public bool ShowInSale { get; set; } = false;
    }

    public class SubcategoryDto
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? IconClass { get; set; }
        public string? ImageUrl { get; set; }
        public int ItemCount { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
    }

    public class SubcategoryListDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string? IconClass { get; set; }
        public int ItemCount { get; set; }
    }

    public class SubcategoryCreateDto
    {
        public int CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? IconClass { get; set; }
        public string? ImageUrl { get; set; }
        public int DisplayOrder { get; set; } = 0;
        public bool IsActive { get; set; } = true;
    }

    public class FeaturedCategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public FeaturedProductDto? FeaturedProduct { get; set; }
        public List<FeaturedSubcategoryDto> Subcategories { get; set; } = new List<FeaturedSubcategoryDto>();
    }

    public class FeaturedProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public string BackgroundColor { get; set; } = "#334155";
    }

    public class FeaturedSubcategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public int ProductCount { get; set; }
        public string BackgroundColor { get; set; } = "#E2E8F0";
    }
} 