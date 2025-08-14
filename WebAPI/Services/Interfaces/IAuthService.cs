using EcommerceAPI.DTOs;

namespace EcommerceAPI.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto?> LoginAsync(UserLoginDto loginDto);
        Task<AuthResponseDto?> RegisterAsync(UserRegisterDto registerDto);
        Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto changePasswordDto);
        Task<bool> ForgotPasswordAsync(string email);
        Task<bool> ResetPasswordAsync(string email, string token, string newPassword);
        string GenerateJwtToken(int userId, string email);
    }
} 