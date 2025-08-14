using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using EcommerceAPI.DTOs;
using EcommerceAPI.Services.Interfaces;

namespace EcommerceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts([FromQuery] PaginationParams paginationParams, 
            [FromQuery] int? categoryId = null, 
            [FromQuery] int? subcategoryId = null, 
            [FromQuery] int? brandId = null)
        {
            var products = await _productService.GetProductsAsync(paginationParams, categoryId, subcategoryId, brandId);
            
            return Ok(new ApiResponse<PagedResult<ProductListDto>>
            {
                Success = true,
                Message = "Products retrieved successfully",
                Data = products
            });
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchProducts([FromQuery] string q, [FromQuery] int? category = null)
        {
            if (string.IsNullOrWhiteSpace(q))
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Search query is required"
                });
            }

            var products = await _productService.SearchProductsAsync(q, category);
            
            return Ok(new ApiResponse<IEnumerable<ProductListDto>>
            {
                Success = true,
                Message = "Search results retrieved successfully",
                Data = products
            });
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            
            if (product == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Product not found"
                });
            }

            // Record product view
            var userId = User.Identity?.IsAuthenticated == true ? 
                int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0") : 
                (int?)null;
            
            await _productService.RecordProductViewAsync(id, userId);

            return Ok(new ApiResponse<ProductDto>
            {
                Success = true,
                Message = "Product retrieved successfully",
                Data = product
            });
        }

        [HttpGet("slug/{slug}")]
        public async Task<IActionResult> GetProductBySlug(string slug)
        {
            var product = await _productService.GetProductBySlugAsync(slug);
            
            if (product == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Product not found"
                });
            }

            // Record product view
            var userId = User.Identity?.IsAuthenticated == true ? 
                int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0") : 
                (int?)null;
            
            await _productService.RecordProductViewAsync(product.Id, userId);

            return Ok(new ApiResponse<ProductDto>
            {
                Success = true,
                Message = "Product retrieved successfully",
                Data = product
            });
        }

        [HttpGet("featured")]
        public async Task<IActionResult> GetFeaturedProducts([FromQuery] int count = 10)
        {
            var products = await _productService.GetFeaturedProductsAsync(count);
            
            return Ok(new ApiResponse<List<ProductListDto>>
            {
                Success = true,
                Message = "Featured products retrieved successfully",
                Data = products
            });
        }

        [HttpGet("new")]
        public async Task<IActionResult> GetNewProducts([FromQuery] int count = 10)
        {
            var products = await _productService.GetNewProductsAsync(count);
            
            return Ok(new ApiResponse<List<ProductListDto>>
            {
                Success = true,
                Message = "New products retrieved successfully",
                Data = products
            });
        }

        [HttpGet("on-sale")]
        public async Task<IActionResult> GetOnSaleProducts([FromQuery] int count = 10)
        {
            var products = await _productService.GetOnSaleProductsAsync(count);
            
            return Ok(new ApiResponse<List<ProductListDto>>
            {
                Success = true,
                Message = "On sale products retrieved successfully",
                Data = products
            });
        }

        [HttpGet("best-sellers")]
        public async Task<IActionResult> GetBestSellerProducts([FromQuery] int count = 10)
        {
            var products = await _productService.GetBestSellerProductsAsync(count);
            
            return Ok(new ApiResponse<List<ProductListDto>>
            {
                Success = true,
                Message = "Best seller products retrieved successfully",
                Data = products
            });
        }

        [HttpGet("popular")]
        public async Task<IActionResult> GetPopularProducts([FromQuery] int count = 10)
        {
            var products = await _productService.GetPopularProductsAsync(count);
            
            return Ok(new ApiResponse<List<ProductListDto>>
            {
                Success = true,
                Message = "Popular products retrieved successfully",
                Data = products
            });
        }

        [HttpGet("{id:int}/related")]
        public async Task<IActionResult> GetRelatedProducts(int id, [FromQuery] int count = 4)
        {
            var products = await _productService.GetRelatedProductsAsync(id, count);
            
            return Ok(new ApiResponse<List<ProductListDto>>
            {
                Success = true,
                Message = "Related products retrieved successfully",
                Data = products
            });
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateProduct([FromBody] ProductCreateDto productDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Invalid product data",
                    Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()
                });
            }

            var product = await _productService.CreateProductAsync(productDto);
            
            return Created($"/api/products/{product.Id}", new ApiResponse<ProductDto>
            {
                Success = true,
                Message = "Product created successfully",
                Data = product
            });
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductUpdateDto productDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Invalid product data",
                    Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()
                });
            }

            productDto.Id = id;
            var product = await _productService.UpdateProductAsync(id, productDto);
            
            if (product == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Product not found"
                });
            }

            return Ok(new ApiResponse<ProductDto>
            {
                Success = true,
                Message = "Product updated successfully",
                Data = product
            });
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var success = await _productService.DeleteProductAsync(id);
            
            if (!success)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Product not found"
                });
            }

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Product deleted successfully"
            });
        }

        [HttpPut("{id:int}/stock")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateStock(int id, [FromBody] UpdateStockDto stockDto)
        {
            var success = await _productService.UpdateStockAsync(id, stockDto.Quantity, stockDto.VariantId);
            
            if (!success)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Product or variant not found"
                });
            }

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Stock updated successfully"
            });
        }
    }

    public class UpdateStockDto
    {
        public int Quantity { get; set; }
        public int? VariantId { get; set; }
    }
} 