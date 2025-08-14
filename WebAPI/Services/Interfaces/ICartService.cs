using EcommerceAPI.DTOs;

namespace EcommerceAPI.Services.Interfaces
{
    public interface ICartService
    {
        Task<CartDto> GetCartAsync(int userId);
        Task<CartItemDto?> AddToCartAsync(int userId, AddToCartDto addToCartDto);
        Task<CartItemDto?> UpdateCartItemAsync(int userId, UpdateCartItemDto updateDto);
        Task<bool> RemoveFromCartAsync(int userId, int cartItemId);
        Task<bool> ClearCartAsync(int userId);
        Task<int> GetCartItemCountAsync(int userId);
    }
} 