using Microsoft.EntityFrameworkCore;
using AutoMapper;
using EcommerceAPI.Data;
using EcommerceAPI.DTOs;
using EcommerceAPI.Models;
using EcommerceAPI.Services.Interfaces;

namespace EcommerceAPI.Services
{
    public class CartService : ICartService
    {
        private readonly EcommerceDbContext _context;
        private readonly IMapper _mapper;

        public CartService(EcommerceDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<CartDto> GetCartAsync(int userId)
        {
            var cartItems = await _context.CartItems
                .Include(ci => ci.Product)
                .Include(ci => ci.Variant)
                .Where(ci => ci.UserId == userId)
                .OrderBy(ci => ci.AddedAt)
                .ToListAsync();

            var cartItemDtos = _mapper.Map<List<CartItemDto>>(cartItems);
            
            // Calculate totals
            decimal subtotal = 0;
            decimal shippingCost = 0;
            
            foreach (var item in cartItems)
            {
                var price = item.Product.Price;
                if (item.Variant != null)
                {
                    price += item.Variant.PriceAdjustment;
                }
                
                subtotal += price * item.Quantity;
                
                if (!item.Product.FreeShipping)
                {
                    shippingCost += item.Product.ShippingCost;
                }
            }

            // Calculate tax (assuming 10% for demo)
            decimal tax = subtotal * 0.10m;
            decimal total = subtotal + shippingCost + tax;

            return new CartDto
            {
                Items = cartItemDtos,
                Subtotal = subtotal,
                ShippingCost = shippingCost,
                Tax = tax,
                Total = total,
                ItemCount = cartItems.Sum(ci => ci.Quantity)
            };
        }

        public async Task<CartItemDto?> AddToCartAsync(int userId, AddToCartDto addToCartDto)
        {
            // Validate product exists and is active
            var product = await _context.Products
                .Include(p => p.Variants)
                .FirstOrDefaultAsync(p => p.Id == addToCartDto.ProductId && p.IsActive);

            if (product == null)
            {
                return null;
            }

            // Validate variant if specified
            if (addToCartDto.VariantId.HasValue)
            {
                var variant = product.Variants.FirstOrDefault(v => v.Id == addToCartDto.VariantId.Value && v.IsActive);
                if (variant == null)
                {
                    return null;
                }
            }

            // Check if item already exists in cart
            var existingCartItem = await _context.CartItems
                .Include(ci => ci.Product)
                .Include(ci => ci.Variant)
                .FirstOrDefaultAsync(ci => 
                    ci.UserId == userId && 
                    ci.ProductId == addToCartDto.ProductId && 
                    ci.VariantId == addToCartDto.VariantId);

            if (existingCartItem != null)
            {
                // Update quantity
                existingCartItem.Quantity += addToCartDto.Quantity;
                existingCartItem.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                // Add new item
                existingCartItem = new CartItem
                {
                    UserId = userId,
                    ProductId = addToCartDto.ProductId,
                    VariantId = addToCartDto.VariantId,
                    Quantity = addToCartDto.Quantity,
                    AddedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                
                _context.CartItems.Add(existingCartItem);
            }

            await _context.SaveChangesAsync();

            // Reload with includes for mapping
            existingCartItem = await _context.CartItems
                .Include(ci => ci.Product)
                .Include(ci => ci.Variant)
                .FirstOrDefaultAsync(ci => ci.Id == existingCartItem.Id);

            return _mapper.Map<CartItemDto>(existingCartItem);
        }

        public async Task<CartItemDto?> UpdateCartItemAsync(int userId, UpdateCartItemDto updateDto)
        {
            var cartItem = await _context.CartItems
                .Include(ci => ci.Product)
                .Include(ci => ci.Variant)
                .FirstOrDefaultAsync(ci => ci.Id == updateDto.CartItemId && ci.UserId == userId);

            if (cartItem == null)
            {
                return null;
            }

            if (updateDto.Quantity <= 0)
            {
                _context.CartItems.Remove(cartItem);
            }
            else
            {
                cartItem.Quantity = updateDto.Quantity;
                cartItem.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            return updateDto.Quantity > 0 ? _mapper.Map<CartItemDto>(cartItem) : null;
        }

        public async Task<bool> RemoveFromCartAsync(int userId, int cartItemId)
        {
            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.Id == cartItemId && ci.UserId == userId);

            if (cartItem == null)
            {
                return false;
            }

            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();
            
            return true;
        }

        public async Task<bool> ClearCartAsync(int userId)
        {
            var cartItems = await _context.CartItems
                .Where(ci => ci.UserId == userId)
                .ToListAsync();

            if (!cartItems.Any())
            {
                return false;
            }

            _context.CartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();
            
            return true;
        }

        public async Task<int> GetCartItemCountAsync(int userId)
        {
            return await _context.CartItems
                .Where(ci => ci.UserId == userId)
                .SumAsync(ci => ci.Quantity);
        }
    }
} 