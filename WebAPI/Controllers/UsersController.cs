using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using EcommerceAPI.DTOs;
using EcommerceAPI.Services.Interfaces;

namespace EcommerceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        private int GetUserId()
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = GetUserId();
            var user = await _userService.GetUserByIdAsync(userId);
            
            if (user == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "User not found"
                });
            }

            return Ok(new ApiResponse<UserDto>
            {
                Success = true,
                Message = "Profile retrieved successfully",
                Data = user
            });
        }

        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UserUpdateDto userUpdateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Invalid user data",
                    Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()
                });
            }

            var userId = GetUserId();
            var user = await _userService.UpdateUserAsync(userId, userUpdateDto);
            
            if (user == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "User not found"
                });
            }

            return Ok(new ApiResponse<UserDto>
            {
                Success = true,
                Message = "Profile updated successfully",
                Data = user
            });
        }

        [HttpGet("addresses")]
        public async Task<IActionResult> GetAddresses()
        {
            var userId = GetUserId();
            var addresses = await _userService.GetUserAddressesAsync(userId);
            
            return Ok(new ApiResponse<List<UserAddressDto>>
            {
                Success = true,
                Message = "Addresses retrieved successfully",
                Data = addresses
            });
        }

        [HttpGet("addresses/{addressId:int}")]
        public async Task<IActionResult> GetAddressById(int addressId)
        {
            var userId = GetUserId();
            var address = await _userService.GetAddressByIdAsync(userId, addressId);
            
            if (address == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Address not found"
                });
            }

            return Ok(new ApiResponse<UserAddressDto>
            {
                Success = true,
                Message = "Address retrieved successfully",
                Data = address
            });
        }

        [HttpPost("addresses")]
        public async Task<IActionResult> CreateAddress([FromBody] CreateAddressDto addressDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Invalid address data",
                    Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()
                });
            }

            var userId = GetUserId();
            var address = await _userService.CreateAddressAsync(userId, addressDto);
            
            return Created($"/api/users/addresses/{address.Id}", new ApiResponse<UserAddressDto>
            {
                Success = true,
                Message = "Address created successfully",
                Data = address
            });
        }

        [HttpPut("addresses/{addressId:int}")]
        public async Task<IActionResult> UpdateAddress(int addressId, [FromBody] CreateAddressDto addressDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Invalid address data",
                    Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()
                });
            }

            var userId = GetUserId();
            var address = await _userService.UpdateAddressAsync(userId, addressId, addressDto);
            
            if (address == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Address not found"
                });
            }

            return Ok(new ApiResponse<UserAddressDto>
            {
                Success = true,
                Message = "Address updated successfully",
                Data = address
            });
        }

        [HttpDelete("addresses/{addressId:int}")]
        public async Task<IActionResult> DeleteAddress(int addressId)
        {
            var userId = GetUserId();
            var success = await _userService.DeleteAddressAsync(userId, addressId);
            
            if (!success)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Address not found"
                });
            }

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Address deleted successfully"
            });
        }

        [HttpPut("addresses/{addressId:int}/default")]
        public async Task<IActionResult> SetDefaultAddress(int addressId)
        {
            var userId = GetUserId();
            var success = await _userService.SetDefaultAddressAsync(userId, addressId);
            
            if (!success)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Address not found"
                });
            }

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Default address set successfully"
            });
        }
    }
} 