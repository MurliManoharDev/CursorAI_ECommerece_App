using Microsoft.EntityFrameworkCore;
using AutoMapper;
using EcommerceAPI.Data;
using EcommerceAPI.DTOs;
using EcommerceAPI.Models;
using EcommerceAPI.Services.Interfaces;

namespace EcommerceAPI.Services
{
    public class UserService : IUserService
    {
        private readonly EcommerceDbContext _context;
        private readonly IMapper _mapper;

        public UserService(EcommerceDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<UserDto?> GetUserByIdAsync(int id)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == id);

            return _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto?> GetUserByEmailAsync(string email)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());

            return _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto?> UpdateUserAsync(int id, UserUpdateDto userDto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return null;
            }

            _mapper.Map(userDto, user);
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return _mapper.Map<UserDto>(user);
        }

        public async Task<List<UserAddressDto>> GetUserAddressesAsync(int userId)
        {
            var addresses = await _context.UserAddresses
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.IsDefault)
                .ThenByDescending(a => a.CreatedAt)
                .ToListAsync();

            return _mapper.Map<List<UserAddressDto>>(addresses);
        }

        public async Task<UserAddressDto?> GetAddressByIdAsync(int userId, int addressId)
        {
            var address = await _context.UserAddresses
                .FirstOrDefaultAsync(a => a.Id == addressId && a.UserId == userId);

            return _mapper.Map<UserAddressDto>(address);
        }

        public async Task<UserAddressDto> CreateAddressAsync(int userId, CreateAddressDto addressDto)
        {
            var address = _mapper.Map<UserAddress>(addressDto);
            address.UserId = userId;
            address.CreatedAt = DateTime.UtcNow;

            // If this is the first address or set as default, make it default
            if (addressDto.IsDefault || !await _context.UserAddresses.AnyAsync(a => a.UserId == userId))
            {
                // Remove default from other addresses
                var existingAddresses = await _context.UserAddresses
                    .Where(a => a.UserId == userId && a.IsDefault)
                    .ToListAsync();

                foreach (var existingAddress in existingAddresses)
                {
                    existingAddress.IsDefault = false;
                }

                address.IsDefault = true;
            }

            _context.UserAddresses.Add(address);
            await _context.SaveChangesAsync();

            return _mapper.Map<UserAddressDto>(address);
        }

        public async Task<UserAddressDto?> UpdateAddressAsync(int userId, int addressId, CreateAddressDto addressDto)
        {
            var address = await _context.UserAddresses
                .FirstOrDefaultAsync(a => a.Id == addressId && a.UserId == userId);

            if (address == null)
            {
                return null;
            }

            _mapper.Map(addressDto, address);

            // Handle default address change
            if (addressDto.IsDefault && !address.IsDefault)
            {
                var existingDefaultAddresses = await _context.UserAddresses
                    .Where(a => a.UserId == userId && a.IsDefault && a.Id != addressId)
                    .ToListAsync();

                foreach (var existingAddress in existingDefaultAddresses)
                {
                    existingAddress.IsDefault = false;
                }
            }

            await _context.SaveChangesAsync();
            return _mapper.Map<UserAddressDto>(address);
        }

        public async Task<bool> DeleteAddressAsync(int userId, int addressId)
        {
            var address = await _context.UserAddresses
                .FirstOrDefaultAsync(a => a.Id == addressId && a.UserId == userId);

            if (address == null)
            {
                return false;
            }

            var wasDefault = address.IsDefault;
            _context.UserAddresses.Remove(address);
            await _context.SaveChangesAsync();

            // If deleted address was default, make the most recent address default
            if (wasDefault)
            {
                var newDefaultAddress = await _context.UserAddresses
                    .Where(a => a.UserId == userId)
                    .OrderByDescending(a => a.CreatedAt)
                    .FirstOrDefaultAsync();

                if (newDefaultAddress != null)
                {
                    newDefaultAddress.IsDefault = true;
                    await _context.SaveChangesAsync();
                }
            }

            return true;
        }

        public async Task<bool> SetDefaultAddressAsync(int userId, int addressId)
        {
            var address = await _context.UserAddresses
                .FirstOrDefaultAsync(a => a.Id == addressId && a.UserId == userId);

            if (address == null)
            {
                return false;
            }

            // Remove default from all other addresses
            var existingDefaultAddresses = await _context.UserAddresses
                .Where(a => a.UserId == userId && a.IsDefault && a.Id != addressId)
                .ToListAsync();

            foreach (var existingAddress in existingDefaultAddresses)
            {
                existingAddress.IsDefault = false;
            }

            address.IsDefault = true;
            await _context.SaveChangesAsync();

            return true;
        }
    }
} 