using EcommerceAPI.DTOs;

namespace EcommerceAPI.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<List<CategoryListDto>> GetAllCategoriesAsync();
        Task<List<CategoryListDto>> GetTopCategoriesAsync();
        Task<List<FeaturedCategoryDto>> GetFeaturedCategoriesAsync();
        Task<CategoryDto?> GetCategoryByIdAsync(int id);
        Task<CategoryDto?> GetCategoryBySlugAsync(string slug);
        Task<List<SubcategoryListDto>> GetSubcategoriesByCategoryIdAsync(int categoryId);
        Task<CategoryDto> CreateCategoryAsync(CategoryCreateDto categoryDto);
        Task<CategoryDto?> UpdateCategoryAsync(int id, CategoryCreateDto categoryDto);
        Task<bool> DeleteCategoryAsync(int id);
        Task<SubcategoryDto> CreateSubcategoryAsync(SubcategoryCreateDto subcategoryDto);
        Task<SubcategoryDto?> UpdateSubcategoryAsync(int id, SubcategoryCreateDto subcategoryDto);
        Task<bool> DeleteSubcategoryAsync(int id);
    }
} 