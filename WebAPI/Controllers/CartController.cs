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
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        private int GetUserId()
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        }

        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var userId = GetUserId();
            var cart = await _cartService.GetCartAsync(userId);
            
            return Ok(new ApiResponse<CartDto>
            {
                Success = true,
                Message = "Cart retrieved successfully",
                Data = cart
            });
        }

        [HttpPost("items")]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartDto addToCartDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Invalid cart item data",
                    Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()
                });
            }

            var userId = GetUserId();
            var cartItem = await _cartService.AddToCartAsync(userId, addToCartDto);
            
            if (cartItem == null)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Product not found or invalid variant"
                });
            }

            return Ok(new ApiResponse<CartItemDto>
            {
                Success = true,
                Message = "Item added to cart successfully",
                Data = cartItem
            });
        }

        [HttpPut("items")]
        public async Task<IActionResult> UpdateCartItem([FromBody] UpdateCartItemDto updateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Invalid update data",
                    Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()
                });
            }

            var userId = GetUserId();
            var cartItem = await _cartService.UpdateCartItemAsync(userId, updateDto);
            
            if (cartItem == null && updateDto.Quantity > 0)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Cart item not found"
                });
            }

            if (updateDto.Quantity <= 0)
            {
                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Item removed from cart successfully"
                });
            }

            return Ok(new ApiResponse<CartItemDto>
            {
                Success = true,
                Message = "Cart item updated successfully",
                Data = cartItem
            });
        }

        [HttpDelete("items/{cartItemId:int}")]
        public async Task<IActionResult> RemoveFromCart(int cartItemId)
        {
            var userId = GetUserId();
            var success = await _cartService.RemoveFromCartAsync(userId, cartItemId);
            
            if (!success)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Cart item not found"
                });
            }

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Item removed from cart successfully"
            });
        }

        [HttpDelete]
        public async Task<IActionResult> ClearCart()
        {
            var userId = GetUserId();
            var success = await _cartService.ClearCartAsync(userId);
            
            if (!success)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Cart is already empty"
                });
            }

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Cart cleared successfully"
            });
        }

        [HttpGet("count")]
        public async Task<IActionResult> GetCartItemCount()
        {
            var userId = GetUserId();
            var count = await _cartService.GetCartItemCountAsync(userId);
            
            return Ok(new ApiResponse<int>
            {
                Success = true,
                Message = "Cart item count retrieved successfully",
                Data = count
            });
        }
    }
} 