using EcommerceAPI.DTOs;

namespace EcommerceAPI.Services.Interfaces
{
    public interface IFrequentlyBoughtTogetherService
    {
        Task<FrequentlyBoughtTogetherResponseDto> GetFrequentlyBoughtTogetherAsync(int productId);
        Task<IEnumerable<FrequentlyBoughtTogetherDto>> GetByProductIdAsync(int productId);
        Task<FrequentlyBoughtTogetherDto> CreateAsync(CreateFrequentlyBoughtTogetherDto dto);
        Task<bool> DeleteAsync(int id);
        Task<bool> UpdateOrderAsync(int productId, List<int> relatedProductIds);
        Task<bool> ProductExistsAsync(int productId);
        Task<List<int>> GetRandomProductIdsAsync(int excludeProductId, int count);
        Task<int> GetCountAsync();
        Task<int> GetRelatedCountForProductAsync(int productId);
    }
} 