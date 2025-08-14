using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using AutoMapper;
using EcommerceAPI.Data;
using EcommerceAPI.DTOs;
using EcommerceAPI.Models;
using EcommerceAPI.Services.Interfaces;

namespace EcommerceAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly EcommerceDbContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public AuthService(EcommerceDbContext context, IMapper mapper, IConfiguration configuration)
        {
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<AuthResponseDto?> LoginAsync(UserLoginDto loginDto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == loginDto.Email.ToLower());

            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            {
                return null;
            }

            if (!user.IsActive)
            {
                return null;
            }

            // Update last login
            user.LastLoginAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            var token = GenerateJwtToken(user.Id, user.Email);
            var userDto = _mapper.Map<UserDto>(user);

            return new AuthResponseDto
            {
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddDays(Convert.ToDouble(_configuration["Jwt:ExpireDays"])),
                User = userDto
            };
        }

        public async Task<AuthResponseDto?> RegisterAsync(UserRegisterDto registerDto)
        {
            // Check if user already exists
            var existingUser = await _context.Users
                .AnyAsync(u => u.Email.ToLower() == registerDto.Email.ToLower());

            if (existingUser)
            {
                return null;
            }

            // Create new user
            var user = _mapper.Map<User>(registerDto);
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);
            user.CreatedAt = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;
            user.IsActive = true;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var token = GenerateJwtToken(user.Id, user.Email);
            var userDto = _mapper.Map<UserDto>(user);

            return new AuthResponseDto
            {
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddDays(Convert.ToDouble(_configuration["Jwt:ExpireDays"])),
                User = userDto
            };
        }

        public async Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto changePasswordDto)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return false;
            }

            // Verify current password
            if (!BCrypt.Net.BCrypt.Verify(changePasswordDto.CurrentPassword, user.PasswordHash))
            {
                return false;
            }

            // Update password
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(changePasswordDto.NewPassword);
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ForgotPasswordAsync(string email)
        {
            Console.WriteLine($"[DEBUG] ForgotPasswordAsync called for email: {email}");
            
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
            
            if (user == null)
            {
                Console.WriteLine($"[DEBUG] User not found for email: {email}");
                // Return true to prevent email enumeration
                return true;
            }

            Console.WriteLine($"[DEBUG] User found: {user.Id} - {user.Email}");

            // Generate reset token
            var resetToken = Guid.NewGuid().ToString();
            Console.WriteLine($"[DEBUG] Generated token: {resetToken}");
            
            var passwordResetToken = new PasswordResetToken
            {
                UserId = user.Id,
                Token = resetToken,
                ExpiresAt = DateTime.UtcNow.AddHours(24), // Token expires in 24 hours
                CreatedAt = DateTime.UtcNow
            };

            try
            {
                _context.PasswordResetTokens.Add(passwordResetToken);
                await _context.SaveChangesAsync();
                Console.WriteLine($"[DEBUG] Token saved to database successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to save token: {ex.Message}");
                Console.WriteLine($"[ERROR] Stack trace: {ex.StackTrace}");
                return false;
            }

            // TODO: Send email with reset link
            // In a real application, you would send an email here
            // For now, we'll just log the reset link
            var resetLink = $"http://localhost:4200/reset-password?token={resetToken}&email={email}";
            Console.WriteLine($"\n===== PASSWORD RESET LINK =====");
            Console.WriteLine($"Password reset link for {email}:");
            Console.WriteLine(resetLink);
            Console.WriteLine($"==============================\n");
            Console.Out.Flush(); // Force output to appear immediately

            return true;
        }

        public async Task<bool> ResetPasswordAsync(string email, string token, string newPassword)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
            
            if (user == null)
            {
                return false;
            }

            var resetToken = await _context.PasswordResetTokens
                .FirstOrDefaultAsync(t => t.UserId == user.Id && 
                                        t.Token == token && 
                                        !t.IsUsed && 
                                        t.ExpiresAt > DateTime.UtcNow);

            if (resetToken == null)
            {
                return false;
            }

            // Update password
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            user.UpdatedAt = DateTime.UtcNow;

            // Mark token as used
            resetToken.IsUsed = true;

            await _context.SaveChangesAsync();
            return true;
        }

        public string GenerateJwtToken(int userId, string email)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured")));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Email, email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(Convert.ToDouble(_configuration["Jwt:ExpireDays"])),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
} 