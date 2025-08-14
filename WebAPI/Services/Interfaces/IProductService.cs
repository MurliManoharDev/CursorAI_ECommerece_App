using EcommerceAPI.DTOs;

namespace EcommerceAPI.Services.Interfaces
{
    public interface IProductService
    {
        Task<PagedResult<ProductListDto>> GetProductsAsync(PaginationParams paginationParams, int? categoryId = null, int? subcategoryId = null, int? brandId = null);
        Task<ProductDto?> GetProductByIdAsync(int id);
        Task<ProductDto?> GetProductBySlugAsync(string slug);
        Task<List<ProductListDto>> GetFeaturedProductsAsync(int count = 10);
        Task<List<ProductListDto>> GetNewProductsAsync(int count = 10);
        Task<List<ProductListDto>> GetOnSaleProductsAsync(int count = 10);
        Task<List<ProductListDto>> GetBestSellerProductsAsync(int count = 10);
        Task<List<ProductListDto>> GetPopularProductsAsync(int count = 10);
        Task<List<ProductListDto>> GetRelatedProductsAsync(int productId, int count = 4);
        Task<IEnumerable<ProductListDto>> SearchProductsAsync(string query, int? categoryId = null);
        Task<ProductDto> CreateProductAsync(ProductCreateDto productDto);
        Task<ProductDto?> UpdateProductAsync(int id, ProductUpdateDto productDto);
        Task<bool> DeleteProductAsync(int id);
        Task<bool> UpdateStockAsync(int productId, int quantity, int? variantId = null);
        Task RecordProductViewAsync(int productId, int? userId = null);
    }
} 