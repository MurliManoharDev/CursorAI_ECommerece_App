using Microsoft.EntityFrameworkCore;
using AutoMapper;
using EcommerceAPI.Data;
using EcommerceAPI.DTOs;
using EcommerceAPI.Models;
using EcommerceAPI.Services.Interfaces;

namespace EcommerceAPI.Services
{
    public class FrequentlyBoughtTogetherService : IFrequentlyBoughtTogetherService
    {
        private readonly EcommerceDbContext _context;
        private readonly IMapper _mapper;

        public FrequentlyBoughtTogetherService(EcommerceDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<FrequentlyBoughtTogetherResponseDto> GetFrequentlyBoughtTogetherAsync(int productId)
        {
            var relatedProducts = await _context.FrequentlyBoughtTogether
                .Where(f => f.ProductId == productId && f.IsActive)
                .Include(f => f.RelatedProduct)
                .OrderBy(f => f.DisplayOrder)
                .Select(f => new FrequentlyBoughtTogetherProductDto
                {
                    Id = f.RelatedProduct.Id,
                    Name = f.RelatedProduct.Name ?? "",
                    Price = f.RelatedProduct.Price,
                    OldPrice = f.RelatedProduct.OldPrice,
                    ImageUrl = f.RelatedProduct.ImageUrl ?? "",
                    Selected = true
                })
                .ToListAsync();

            // Get the main product
            var mainProduct = await _context.Products
                .Where(p => p.Id == productId)
                .Select(p => new FrequentlyBoughtTogetherProductDto
                {
                    Id = p.Id,
                    Name = p.Name ?? "",
                    Price = p.Price,
                    OldPrice = p.OldPrice,
                    ImageUrl = p.ImageUrl ?? "",
                    Selected = true
                })
                .FirstOrDefaultAsync();

            // If no main product found, return empty response
            if (mainProduct == null)
            {
                return new FrequentlyBoughtTogetherResponseDto
                {
                    Products = new List<FrequentlyBoughtTogetherProductDto>(),
                    TotalPrice = 0,
                    TotalOldPrice = 0,
                    TotalSavings = 0
                };
            }

            // Always include the main product first
            relatedProducts.Insert(0, mainProduct);

            var response = new FrequentlyBoughtTogetherResponseDto
            {
                Products = relatedProducts,
                TotalPrice = relatedProducts.Where(p => p.Selected).Sum(p => p.Price),
                TotalOldPrice = relatedProducts.Where(p => p.Selected && p.OldPrice.HasValue).Sum(p => p.OldPrice.Value),
            };

            response.TotalSavings = response.TotalOldPrice > 0 ? response.TotalOldPrice - response.TotalPrice : 0;

            return response;
        }

        public async Task<IEnumerable<FrequentlyBoughtTogetherDto>> GetByProductIdAsync(int productId)
        {
            var items = await _context.FrequentlyBoughtTogether
                .Where(f => f.ProductId == productId)
                .Include(f => f.RelatedProduct)
                .OrderBy(f => f.DisplayOrder)
                .ToListAsync();

            return _mapper.Map<IEnumerable<FrequentlyBoughtTogetherDto>>(items);
        }

        public async Task<FrequentlyBoughtTogetherDto> CreateAsync(CreateFrequentlyBoughtTogetherDto dto)
        {
            var items = new List<FrequentlyBoughtTogether>();
            
            for (int i = 0; i < dto.RelatedProductIds.Length; i++)
            {
                var item = new FrequentlyBoughtTogether
                {
                    ProductId = dto.ProductId,
                    RelatedProductId = dto.RelatedProductIds[i],
                    DisplayOrder = i,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };
                items.Add(item);
            }

            _context.FrequentlyBoughtTogether.AddRange(items);
            await _context.SaveChangesAsync();

            // Return the first created item
            return _mapper.Map<FrequentlyBoughtTogetherDto>(items.FirstOrDefault());
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var item = await _context.FrequentlyBoughtTogether.FindAsync(id);
            if (item == null)
                return false;

            _context.FrequentlyBoughtTogether.Remove(item);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateOrderAsync(int productId, List<int> relatedProductIds)
        {
            // Remove existing relationships
            var existingItems = await _context.FrequentlyBoughtTogether
                .Where(f => f.ProductId == productId)
                .ToListAsync();

            _context.FrequentlyBoughtTogether.RemoveRange(existingItems);

            // Add new relationships with updated order
            for (int i = 0; i < relatedProductIds.Count; i++)
            {
                var item = new FrequentlyBoughtTogether
                {
                    ProductId = productId,
                    RelatedProductId = relatedProductIds[i],
                    DisplayOrder = i,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };
                _context.FrequentlyBoughtTogether.Add(item);
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ProductExistsAsync(int productId)
        {
            return await _context.Products.AnyAsync(p => p.Id == productId);
        }

        public async Task<List<int>> GetRandomProductIdsAsync(int excludeProductId, int count)
        {
            var availableProductIds = await _context.Products
                .Where(p => p.Id != excludeProductId)
                .Select(p => p.Id)
                .ToListAsync();

            if (!availableProductIds.Any())
                return new List<int>();

            var random = new Random();
            var selectedIds = new List<int>();

            for (int i = 0; i < count && i < availableProductIds.Count; i++)
            {
                var index = random.Next(availableProductIds.Count);
                selectedIds.Add(availableProductIds[index]);
                availableProductIds.RemoveAt(index);
            }

            return selectedIds;
        }

        public async Task<int> GetCountAsync()
        {
            return await _context.FrequentlyBoughtTogether.CountAsync();
        }

        public async Task<int> GetRelatedCountForProductAsync(int productId)
        {
            return await _context.FrequentlyBoughtTogether
                .Where(f => f.ProductId == productId && f.IsActive)
                .CountAsync();
        }
    }
} 