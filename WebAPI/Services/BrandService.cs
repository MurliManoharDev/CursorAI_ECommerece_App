using Microsoft.EntityFrameworkCore;
using AutoMapper;
using EcommerceAPI.Data;
using EcommerceAPI.DTOs;
using EcommerceAPI.Models;
using EcommerceAPI.Services.Interfaces;

namespace EcommerceAPI.Services
{
    public class BrandService : IBrandService
    {
        private readonly EcommerceDbContext _context;
        private readonly IMapper _mapper;

        public BrandService(EcommerceDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<BrandDto>> GetAllBrandsAsync()
        {
            var brands = await _context.Brands
                .Include(b => b.Products)
                .OrderBy(b => b.DisplayOrder)
                .ThenBy(b => b.Name)
                .ToListAsync();

            return _mapper.Map<List<BrandDto>>(brands);
        }

        public async Task<List<BrandDto>> GetFeaturedBrandsAsync()
        {
            var brands = await _context.Brands
                .Include(b => b.Products)
                .Where(b => b.IsFeatured)
                .OrderBy(b => b.DisplayOrder)
                .ThenBy(b => b.Name)
                .ToListAsync();

            return _mapper.Map<List<BrandDto>>(brands);
        }

        public async Task<BrandDto?> GetBrandByIdAsync(int id)
        {
            var brand = await _context.Brands
                .Include(b => b.Products)
                .FirstOrDefaultAsync(b => b.Id == id);

            return _mapper.Map<BrandDto>(brand);
        }

        public async Task<BrandDto> CreateBrandAsync(BrandDto brandDto)
        {
            var brand = new Brand
            {
                Name = brandDto.Name,
                LogoUrl = brandDto.LogoUrl,
                Description = brandDto.Description,
                IsFeatured = brandDto.IsFeatured,
                DisplayOrder = brandDto.DisplayOrder,
                CreatedAt = DateTime.UtcNow
            };

            _context.Brands.Add(brand);
            await _context.SaveChangesAsync();

            return await GetBrandByIdAsync(brand.Id) ?? throw new Exception("Failed to create brand");
        }

        public async Task<BrandDto?> UpdateBrandAsync(int id, BrandDto brandDto)
        {
            var brand = await _context.Brands.FindAsync(id);
            if (brand == null)
            {
                return null;
            }

            brand.Name = brandDto.Name;
            brand.LogoUrl = brandDto.LogoUrl;
            brand.Description = brandDto.Description;
            brand.IsFeatured = brandDto.IsFeatured;
            brand.DisplayOrder = brandDto.DisplayOrder;

            await _context.SaveChangesAsync();
            return await GetBrandByIdAsync(id);
        }

        public async Task<bool> DeleteBrandAsync(int id)
        {
            var brand = await _context.Brands
                .Include(b => b.Products)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (brand == null)
            {
                return false;
            }

            // Don't delete if brand has products
            if (brand.Products.Any())
            {
                return false;
            }

            _context.Brands.Remove(brand);
            await _context.SaveChangesAsync();
            
            return true;
        }
    }
} 