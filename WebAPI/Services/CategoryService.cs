using Microsoft.EntityFrameworkCore;
using AutoMapper;
using EcommerceAPI.Data;
using EcommerceAPI.DTOs;
using EcommerceAPI.Models;
using EcommerceAPI.Services.Interfaces;

namespace EcommerceAPI.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly EcommerceDbContext _context;
        private readonly IMapper _mapper;

        public CategoryService(EcommerceDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<CategoryListDto>> GetAllCategoriesAsync()
        {
            var categories = await _context.Categories
                .Include(c => c.Subcategories)
                .Include(c => c.Products)
                .Where(c => c.IsActive && c.ParentId == null)
                .OrderBy(c => c.DisplayOrder)
                .ThenBy(c => c.Name)
                .ToListAsync();

            var categoryDtos = _mapper.Map<List<CategoryListDto>>(categories);

            // Map subcategories for each category
            foreach (var categoryDto in categoryDtos)
            {
                var category = categories.First(c => c.Id == categoryDto.Id);
                categoryDto.Subcategories = _mapper.Map<List<SubcategoryListDto>>(
                    category.Subcategories.Where(s => s.IsActive).OrderBy(s => s.DisplayOrder)
                );
            }

            return categoryDtos;
        }

        public async Task<List<CategoryListDto>> GetTopCategoriesAsync()
        {
            var categories = await _context.Categories
                .Include(c => c.Products)
                .Where(c => c.IsActive && c.ShowInMenu && c.ParentId == null)
                .OrderBy(c => c.DisplayOrder)
                .ThenBy(c => c.Name)
                .Take(6)
                .ToListAsync();

            return _mapper.Map<List<CategoryListDto>>(categories);
        }

        public async Task<List<FeaturedCategoryDto>> GetFeaturedCategoriesAsync()
        {
            // Get top 3 categories for the featured sections
            var categories = await _context.Categories
                .Include(c => c.Subcategories)
                    .ThenInclude(s => s.Products)
                .Include(c => c.Products)
                .Where(c => c.IsActive && c.ParentId == null)
                .OrderBy(c => c.DisplayOrder)
                .Take(3)
                .ToListAsync();

            var featuredCategories = new List<FeaturedCategoryDto>();

            foreach (var category in categories)
            {
                var featuredCategory = new FeaturedCategoryDto
                {
                    Id = category.Id,
                    Name = category.Name,
                    Slug = category.Slug,
                    Subcategories = new List<FeaturedSubcategoryDto>()
                };

                // Get a featured product from this category
                var featuredProduct = category.Products
                    .Where(p => p.IsActive && p.IsFeatured)
                    .OrderByDescending(p => p.CreatedAt)
                    .FirstOrDefault();

                if (featuredProduct == null)
                {
                    // If no featured product, get the latest product
                    featuredProduct = category.Products
                        .Where(p => p.IsActive)
                        .OrderByDescending(p => p.CreatedAt)
                        .FirstOrDefault();
                }

                if (featuredProduct != null)
                {
                    featuredCategory.FeaturedProduct = new FeaturedProductDto
                    {
                        Id = featuredProduct.Id,
                        Name = featuredProduct.Name,
                        Description = featuredProduct.Subtitle ?? featuredProduct.Description,
                        ImageUrl = featuredProduct.ImageUrl ?? "/assets/images/placeholder.png",
                        BackgroundColor = GetCategoryColor(category.Name)
                    };
                }

                // Get top 4 subcategories with product counts
                var subcategories = category.Subcategories
                    .Where(s => s.IsActive)
                    .OrderBy(s => s.DisplayOrder)
                    .Take(4)
                    .Select(s => new FeaturedSubcategoryDto
                    {
                        Id = s.Id,
                        Name = s.Name,
                        Slug = s.Slug,
                        ProductCount = s.Products.Count(p => p.IsActive),
                        ImageUrl = s.Products
                            .Where(p => p.IsActive)
                            .OrderByDescending(p => p.CreatedAt)
                            .Select(p => p.ImageUrl)
                            .FirstOrDefault() ?? "/assets/images/placeholder.png",
                        BackgroundColor = "#E2E8F0"
                    })
                    .ToList();

                featuredCategory.Subcategories = subcategories;
                featuredCategories.Add(featuredCategory);
            }

            return featuredCategories;
        }

        private string GetCategoryColor(string categoryName)
        {
            // Return different colors based on category
            return categoryName.ToLower() switch
            {
                var name when name.Contains("audio") || name.Contains("camera") => "#334155",
                var name when name.Contains("gaming") => "#F1F5F9",
                var name when name.Contains("office") => "#374151",
                var name when name.Contains("computer") => "#1E293B",
                var name when name.Contains("mobile") => "#0F172A",
                _ => "#475569"
            };
        }

        public async Task<CategoryDto?> GetCategoryByIdAsync(int id)
        {
            var category = await _context.Categories
                .Include(c => c.Parent)
                .Include(c => c.Subcategories)
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                return null;
            }

            var categoryDto = _mapper.Map<CategoryDto>(category);
            categoryDto.Subcategories = _mapper.Map<List<SubcategoryDto>>(
                category.Subcategories.Where(s => s.IsActive).OrderBy(s => s.DisplayOrder)
            );

            return categoryDto;
        }

        public async Task<CategoryDto?> GetCategoryBySlugAsync(string slug)
        {
            var category = await _context.Categories
                .Include(c => c.Parent)
                .Include(c => c.Subcategories)
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Slug == slug && c.IsActive);

            if (category == null)
            {
                return null;
            }

            var categoryDto = _mapper.Map<CategoryDto>(category);
            categoryDto.Subcategories = _mapper.Map<List<SubcategoryDto>>(
                category.Subcategories.Where(s => s.IsActive).OrderBy(s => s.DisplayOrder)
            );

            return categoryDto;
        }

        public async Task<List<SubcategoryListDto>> GetSubcategoriesByCategoryIdAsync(int categoryId)
        {
            var subcategories = await _context.Subcategories
                .Where(s => s.CategoryId == categoryId && s.IsActive)
                .OrderBy(s => s.DisplayOrder)
                .ThenBy(s => s.Name)
                .ToListAsync();

            return _mapper.Map<List<SubcategoryListDto>>(subcategories);
        }

        public async Task<CategoryDto> CreateCategoryAsync(CategoryCreateDto categoryDto)
        {
            var category = _mapper.Map<Category>(categoryDto);
            category.CreatedAt = DateTime.UtcNow;

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return await GetCategoryByIdAsync(category.Id) ?? throw new Exception("Failed to create category");
        }

        public async Task<CategoryDto?> UpdateCategoryAsync(int id, CategoryCreateDto categoryDto)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return null;
            }

            _mapper.Map(categoryDto, category);
            await _context.SaveChangesAsync();

            return await GetCategoryByIdAsync(id);
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var category = await _context.Categories
                .Include(c => c.Subcategories)
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                return false;
            }

            // Check if category has products
            if (category.Products.Any() || category.Subcategories.Any(s => s.Products.Any()))
            {
                // Soft delete
                category.IsActive = false;
                foreach (var subcategory in category.Subcategories)
                {
                    subcategory.IsActive = false;
                }
            }
            else
            {
                // Hard delete if no products
                _context.Categories.Remove(category);
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<SubcategoryDto> CreateSubcategoryAsync(SubcategoryCreateDto subcategoryDto)
        {
            var subcategory = _mapper.Map<Subcategory>(subcategoryDto);
            subcategory.CreatedAt = DateTime.UtcNow;

            _context.Subcategories.Add(subcategory);
            await _context.SaveChangesAsync();

            return _mapper.Map<SubcategoryDto>(subcategory);
        }

        public async Task<SubcategoryDto?> UpdateSubcategoryAsync(int id, SubcategoryCreateDto subcategoryDto)
        {
            var subcategory = await _context.Subcategories.FindAsync(id);
            if (subcategory == null)
            {
                return null;
            }

            _mapper.Map(subcategoryDto, subcategory);
            await _context.SaveChangesAsync();

            return _mapper.Map<SubcategoryDto>(subcategory);
        }

        public async Task<bool> DeleteSubcategoryAsync(int id)
        {
            var subcategory = await _context.Subcategories
                .Include(s => s.Products)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (subcategory == null)
            {
                return false;
            }

            if (subcategory.Products.Any())
            {
                // Soft delete if has products
                subcategory.IsActive = false;
            }
            else
            {
                // Hard delete if no products
                _context.Subcategories.Remove(subcategory);
            }

            await _context.SaveChangesAsync();
            return true;
        }
    }
} 