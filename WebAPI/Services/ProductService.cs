using Microsoft.EntityFrameworkCore;
using AutoMapper;
using EcommerceAPI.Data;
using EcommerceAPI.DTOs;
using EcommerceAPI.Models;
using EcommerceAPI.Services.Interfaces;

namespace EcommerceAPI.Services
{
    public class ProductService : IProductService
    {
        private readonly EcommerceDbContext _context;
        private readonly IMapper _mapper;

        public ProductService(EcommerceDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PagedResult<ProductListDto>> GetProductsAsync(PaginationParams paginationParams, int? categoryId = null, int? subcategoryId = null, int? brandId = null)
        {
            var query = _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Include(p => p.Reviews)
                .Where(p => p.IsActive);

            // Apply filters
            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            if (subcategoryId.HasValue)
            {
                query = query.Where(p => p.SubcategoryId == subcategoryId.Value);
            }

            if (brandId.HasValue)
            {
                query = query.Where(p => p.BrandId == brandId.Value);
            }

            // Apply search
            if (!string.IsNullOrWhiteSpace(paginationParams.SearchTerm))
            {
                var searchTerm = paginationParams.SearchTerm.ToLower();
                query = query.Where(p => 
                    p.Name.ToLower().Contains(searchTerm) ||
                    (p.Description != null && p.Description.ToLower().Contains(searchTerm)) ||
                    (p.Sku != null && p.Sku.ToLower().Contains(searchTerm))
                );
            }

            // Get total count
            var totalCount = await query.CountAsync();

            // Apply sorting
            query = ApplySorting(query, paginationParams.SortBy, paginationParams.SortDescending);

            // Apply pagination
            var products = await query
                .Skip((paginationParams.PageNumber - 1) * paginationParams.PageSize)
                .Take(paginationParams.PageSize)
                .ToListAsync();

            var productDtos = _mapper.Map<List<ProductListDto>>(products);

            return new PagedResult<ProductListDto>
            {
                Items = productDtos,
                TotalCount = totalCount,
                PageNumber = paginationParams.PageNumber,
                PageSize = paginationParams.PageSize
            };
        }

        public async Task<ProductDto?> GetProductByIdAsync(int id)
        {
            var product = await _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Include(p => p.Subcategory)
                .Include(p => p.Variants)
                .Include(p => p.Tags)
                .Include(p => p.Reviews)
                .FirstOrDefaultAsync(p => p.Id == id && p.IsActive);

            return _mapper.Map<ProductDto>(product);
        }

        public async Task<ProductDto?> GetProductBySlugAsync(string slug)
        {
            var product = await _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Include(p => p.Subcategory)
                .Include(p => p.Variants)
                .Include(p => p.Tags)
                .Include(p => p.Reviews)
                .FirstOrDefaultAsync(p => p.Slug == slug && p.IsActive);

            return _mapper.Map<ProductDto>(product);
        }

        public async Task<List<ProductListDto>> GetFeaturedProductsAsync(int count = 10)
        {
            var products = await _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Include(p => p.Reviews)
                .Where(p => p.IsActive && p.IsFeatured)
                .OrderByDescending(p => p.CreatedAt)
                .Take(count)
                .ToListAsync();

            return _mapper.Map<List<ProductListDto>>(products);
        }

        public async Task<List<ProductListDto>> GetNewProductsAsync(int count = 10)
        {
            var products = await _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Include(p => p.Reviews)
                .Where(p => p.IsActive && p.IsNew)
                .OrderByDescending(p => p.CreatedAt)
                .Take(count)
                .ToListAsync();

            return _mapper.Map<List<ProductListDto>>(products);
        }

        public async Task<List<ProductListDto>> GetOnSaleProductsAsync(int count = 10)
        {
            var products = await _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Include(p => p.Reviews)
                .Where(p => p.IsActive && p.IsOnSale && p.OldPrice.HasValue)
                // Cast decimal to double for SQLite compatibility
                .OrderByDescending(p => ((double)p.OldPrice.Value - (double)p.Price) / (double)p.OldPrice.Value)
                .Take(count)
                .ToListAsync();

            return _mapper.Map<List<ProductListDto>>(products);
        }

        public async Task<List<ProductListDto>> GetBestSellerProductsAsync(int count = 10)
        {
            var products = await _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Include(p => p.Reviews)
                .Where(p => p.IsActive)
                .OrderByDescending(p => p.SalesCount)
                .ThenByDescending(p => p.ViewsCount)
                .Take(count)
                .ToListAsync();

            return _mapper.Map<List<ProductListDto>>(products);
        }

        public async Task<List<ProductListDto>> GetPopularProductsAsync(int count = 10)
        {
            var products = await _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Include(p => p.Reviews)
                .Where(p => p.IsActive)
                .OrderByDescending(p => p.ViewsCount)
                .ThenByDescending(p => p.SalesCount)
                .Take(count)
                .ToListAsync();

            return _mapper.Map<List<ProductListDto>>(products);
        }

        public async Task<List<ProductListDto>> GetRelatedProductsAsync(int productId, int count = 4)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return new List<ProductListDto>();
            }

            // First get more products than needed to allow for randomization
            var candidateProducts = await _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Include(p => p.Reviews)
                .Where(p => p.IsActive && 
                           p.Id != productId && 
                           (p.CategoryId == product.CategoryId || p.SubcategoryId == product.SubcategoryId))
                .OrderByDescending(p => p.ViewsCount) // Order by popularity first
                .Take(count * 3) // Get 3x the requested amount
                .ToListAsync();

            // Randomize in memory and take the requested count
            var random = new Random();
            var relatedProducts = candidateProducts
                .OrderBy(p => random.Next())
                .Take(count)
                .ToList();

            return _mapper.Map<List<ProductListDto>>(relatedProducts);
        }

        public async Task<IEnumerable<ProductListDto>> SearchProductsAsync(string query, int? categoryId = null)
        {
            var searchQuery = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Subcategory)
                .Include(p => p.Brand)
                .Where(p => p.IsActive);

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(query))
            {
                var searchTerm = query.ToLower();
                searchQuery = searchQuery.Where(p => 
                    p.Name.ToLower().Contains(searchTerm) ||
                    p.Description.ToLower().Contains(searchTerm) ||
                    (p.Subtitle != null && p.Subtitle.ToLower().Contains(searchTerm)) ||
                    p.Brand.Name.ToLower().Contains(searchTerm) ||
                    p.Category.Name.ToLower().Contains(searchTerm) ||
                    (p.Subcategory != null && p.Subcategory.Name.ToLower().Contains(searchTerm))
                );
            }

            // Apply category filter
            if (categoryId.HasValue)
            {
                searchQuery = searchQuery.Where(p => p.CategoryId == categoryId.Value);
            }

            // Order by relevance (products with name match first, then by popularity)
            var products = await searchQuery
                .OrderByDescending(p => p.Name.ToLower().StartsWith(query.ToLower()))
                .ThenByDescending(p => p.ViewsCount)
                .Take(20) // Limit results
                .ToListAsync();

            return _mapper.Map<List<ProductListDto>>(products);
        }

        public async Task<ProductDto> CreateProductAsync(ProductCreateDto productDto)
        {
            var product = _mapper.Map<Product>(productDto);
            product.CreatedAt = DateTime.UtcNow;
            product.UpdatedAt = DateTime.UtcNow;

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return await GetProductByIdAsync(product.Id) ?? throw new Exception("Failed to create product");
        }

        public async Task<ProductDto?> UpdateProductAsync(int id, ProductUpdateDto productDto)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return null;
            }

            _mapper.Map(productDto, product);
            product.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return await GetProductByIdAsync(id);
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return false;
            }

            product.IsActive = false;
            product.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateStockAsync(int productId, int quantity, int? variantId = null)
        {
            if (variantId.HasValue)
            {
                var variant = await _context.ProductVariants
                    .FirstOrDefaultAsync(v => v.Id == variantId.Value && v.ProductId == productId);
                
                if (variant == null)
                {
                    return false;
                }

                variant.StockQuantity = quantity;
            }
            else
            {
                var product = await _context.Products.FindAsync(productId);
                if (product == null)
                {
                    return false;
                }

                product.StockQuantity = quantity;
                product.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task RecordProductViewAsync(int productId, int? userId = null)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return;
            }

            // Increment view count
            product.ViewsCount++;
            product.UpdatedAt = DateTime.UtcNow;

            // Record in recently viewed if user is logged in
            if (userId.HasValue)
            {
                var recentlyViewed = await _context.RecentlyViewed
                    .FirstOrDefaultAsync(rv => rv.UserId == userId.Value && rv.ProductId == productId);

                if (recentlyViewed != null)
                {
                    recentlyViewed.ViewedAt = DateTime.UtcNow;
                }
                else
                {
                    _context.RecentlyViewed.Add(new RecentlyViewed
                    {
                        UserId = userId.Value,
                        ProductId = productId,
                        ViewedAt = DateTime.UtcNow
                    });
                }
            }

            await _context.SaveChangesAsync();
        }

        private IQueryable<Product> ApplySorting(IQueryable<Product> query, string? sortBy, bool sortDescending)
        {
            sortBy = sortBy?.ToLower() ?? "name";

            query = sortBy switch
            {
                // Cast decimal to double for SQLite compatibility
                "price" => sortDescending ? query.OrderByDescending(p => (double)p.Price) : query.OrderBy(p => (double)p.Price),
                "date" => sortDescending ? query.OrderByDescending(p => p.CreatedAt) : query.OrderBy(p => p.CreatedAt),
                "popularity" => sortDescending ? query.OrderByDescending(p => p.ViewsCount) : query.OrderBy(p => p.ViewsCount),
                "sales" => sortDescending ? query.OrderByDescending(p => p.SalesCount) : query.OrderBy(p => p.SalesCount),
                _ => sortDescending ? query.OrderByDescending(p => p.Name) : query.OrderBy(p => p.Name)
            };

            return query;
        }
    }
} 