using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EcommerceAPI.DTOs;
using EcommerceAPI.Services.Interfaces;
using EcommerceAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly EcommerceDbContext _context;

        public CategoriesController(ICategoryService categoryService, EcommerceDbContext context)
        {
            _categoryService = categoryService;
            _context = context;
        }

        // TEMPORARY: Database diagnostic endpoint
        [HttpGet("db-test")]
        public IActionResult TestDatabase()
        {
            try
            {
                var connectionString = _context.Database.GetDbConnection().ConnectionString;
                var dbPath = connectionString?.Replace("Data Source=", "");
                
                var dbExists = !string.IsNullOrEmpty(dbPath) && System.IO.File.Exists(dbPath);
                var homeDbExists = System.IO.File.Exists("/home/ecommerce.db");
                var localDbExists = System.IO.File.Exists("ecommerce.db");
                var wwwrootDbExists = System.IO.File.Exists("/home/site/wwwroot/ecommerce.db");
                
                // Try to count records
                int categoryCount = 0;
                int productCount = 0;
                string error = null;
                
                try
                {
                    categoryCount = _context.Categories.Count();
                    productCount = _context.Products.Count();
                }
                catch (Exception ex)
                {
                    error = ex.Message;
                }
                
                // Check write permissions
                bool canWriteHome = false;
                bool canWriteLocal = false;
                
                try
                {
                    System.IO.File.WriteAllText("/home/test.txt", "test");
                    System.IO.File.Delete("/home/test.txt");
                    canWriteHome = true;
                }
                catch { }
                
                try
                {
                    System.IO.File.WriteAllText("test.txt", "test");
                    System.IO.File.Delete("test.txt");
                    canWriteLocal = true;
                }
                catch { }
                
                return Ok(new
                {
                    environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"),
                    connectionString = connectionString,
                    dbPath = dbPath,
                    dbExists = dbExists,
                    homeDbExists = homeDbExists,
                    localDbExists = localDbExists,
                    wwwrootDbExists = wwwrootDbExists,
                    categoryCount = categoryCount,
                    productCount = productCount,
                    error = error,
                    canWriteHome = canWriteHome,
                    canWriteLocal = canWriteLocal,
                    currentDirectory = Directory.GetCurrentDirectory()
                });
            }
            catch (Exception ex)
            {
                return Ok(new { mainError = ex.Message, stackTrace = ex.StackTrace });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            
            return Ok(new ApiResponse<List<CategoryListDto>>
            {
                Success = true,
                Message = "Categories retrieved successfully",
                Data = categories
            });
        }

        [HttpGet("top")]
        public async Task<IActionResult> GetTopCategories()
        {
            var categories = await _categoryService.GetTopCategoriesAsync();
            
            return Ok(new ApiResponse<List<CategoryListDto>>
            {
                Success = true,
                Message = "Top categories retrieved successfully",
                Data = categories
            });
        }

        [HttpGet("featured")]
        public async Task<IActionResult> GetFeaturedCategories()
        {
            var categories = await _categoryService.GetFeaturedCategoriesAsync();
            
            return Ok(new ApiResponse<List<FeaturedCategoryDto>>
            {
                Success = true,
                Message = "Featured categories retrieved successfully",
                Data = categories
            });
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            
            if (category == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Category not found"
                });
            }

            return Ok(new ApiResponse<CategoryDto>
            {
                Success = true,
                Message = "Category retrieved successfully",
                Data = category
            });
        }

        [HttpGet("slug/{slug}")]
        public async Task<IActionResult> GetCategoryBySlug(string slug)
        {
            var category = await _categoryService.GetCategoryBySlugAsync(slug);
            
            if (category == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Category not found"
                });
            }

            return Ok(new ApiResponse<CategoryDto>
            {
                Success = true,
                Message = "Category retrieved successfully",
                Data = category
            });
        }

        [HttpGet("{categoryId:int}/subcategories")]
        public async Task<IActionResult> GetSubcategoriesByCategoryId(int categoryId)
        {
            var subcategories = await _categoryService.GetSubcategoriesByCategoryIdAsync(categoryId);
            
            return Ok(new ApiResponse<List<SubcategoryListDto>>
            {
                Success = true,
                Message = "Subcategories retrieved successfully",
                Data = subcategories
            });
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryCreateDto categoryDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Invalid category data",
                    Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()
                });
            }

            var category = await _categoryService.CreateCategoryAsync(categoryDto);
            
            return Created($"/api/categories/{category.Id}", new ApiResponse<CategoryDto>
            {
                Success = true,
                Message = "Category created successfully",
                Data = category
            });
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] CategoryCreateDto categoryDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Invalid category data",
                    Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()
                });
            }

            var category = await _categoryService.UpdateCategoryAsync(id, categoryDto);
            
            if (category == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Category not found"
                });
            }

            return Ok(new ApiResponse<CategoryDto>
            {
                Success = true,
                Message = "Category updated successfully",
                Data = category
            });
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var success = await _categoryService.DeleteCategoryAsync(id);
            
            if (!success)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Category not found"
                });
            }

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Category deleted successfully"
            });
        }

        [HttpPost("subcategories")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateSubcategory([FromBody] SubcategoryCreateDto subcategoryDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Invalid subcategory data",
                    Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()
                });
            }

            var subcategory = await _categoryService.CreateSubcategoryAsync(subcategoryDto);
            
            return Created($"/api/categories/subcategories/{subcategory.Id}", new ApiResponse<SubcategoryDto>
            {
                Success = true,
                Message = "Subcategory created successfully",
                Data = subcategory
            });
        }

        [HttpPut("subcategories/{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateSubcategory(int id, [FromBody] SubcategoryCreateDto subcategoryDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Invalid subcategory data",
                    Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()
                });
            }

            var subcategory = await _categoryService.UpdateSubcategoryAsync(id, subcategoryDto);
            
            if (subcategory == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Subcategory not found"
                });
            }

            return Ok(new ApiResponse<SubcategoryDto>
            {
                Success = true,
                Message = "Subcategory updated successfully",
                Data = subcategory
            });
        }

        [HttpDelete("subcategories/{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteSubcategory(int id)
        {
            var success = await _categoryService.DeleteSubcategoryAsync(id);
            
            if (!success)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Subcategory not found"
                });
            }

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Subcategory deleted successfully"
            });
        }
    }
} 