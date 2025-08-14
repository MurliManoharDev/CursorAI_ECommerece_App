using EcommerceAPI.DTOs;

namespace EcommerceAPI.Services.Interfaces
{
    public interface IBrandService
    {
        Task<List<BrandDto>> GetAllBrandsAsync();
        Task<List<BrandDto>> GetFeaturedBrandsAsync();
        Task<BrandDto?> GetBrandByIdAsync(int id);
        Task<BrandDto> CreateBrandAsync(BrandDto brandDto);
        Task<BrandDto?> UpdateBrandAsync(int id, BrandDto brandDto);
        Task<bool> DeleteBrandAsync(int id);
    }
} 