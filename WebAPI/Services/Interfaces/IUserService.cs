using EcommerceAPI.DTOs;

namespace EcommerceAPI.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserDto?> GetUserByIdAsync(int id);
        Task<UserDto?> GetUserByEmailAsync(string email);
        Task<UserDto?> UpdateUserAsync(int id, UserUpdateDto userDto);
        Task<List<UserAddressDto>> GetUserAddressesAsync(int userId);
        Task<UserAddressDto?> GetAddressByIdAsync(int userId, int addressId);
        Task<UserAddressDto> CreateAddressAsync(int userId, CreateAddressDto addressDto);
        Task<UserAddressDto?> UpdateAddressAsync(int userId, int addressId, CreateAddressDto addressDto);
        Task<bool> DeleteAddressAsync(int userId, int addressId);
        Task<bool> SetDefaultAddressAsync(int userId, int addressId);
    }
} 