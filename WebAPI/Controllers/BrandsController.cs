using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EcommerceAPI.DTOs;
using EcommerceAPI.Services.Interfaces;

namespace EcommerceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BrandsController : ControllerBase
    {
        private readonly IBrandService _brandService;

        public BrandsController(IBrandService brandService)
        {
            _brandService = brandService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBrands()
        {
            var brands = await _brandService.GetAllBrandsAsync();
            
            return Ok(new ApiResponse<List<BrandDto>>
            {
                Success = true,
                Message = "Brands retrieved successfully",
                Data = brands
            });
        }

        [HttpGet("featured")]
        public async Task<IActionResult> GetFeaturedBrands()
        {
            var brands = await _brandService.GetFeaturedBrandsAsync();
            
            return Ok(new ApiResponse<List<BrandDto>>
            {
                Success = true,
                Message = "Featured brands retrieved successfully",
                Data = brands
            });
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetBrandById(int id)
        {
            var brand = await _brandService.GetBrandByIdAsync(id);
            
            if (brand == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Brand not found"
                });
            }

            return Ok(new ApiResponse<BrandDto>
            {
                Success = true,
                Message = "Brand retrieved successfully",
                Data = brand
            });
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateBrand([FromBody] BrandDto brandDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Invalid brand data",
                    Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()
                });
            }

            var brand = await _brandService.CreateBrandAsync(brandDto);
            
            return Created($"/api/brands/{brand.Id}", new ApiResponse<BrandDto>
            {
                Success = true,
                Message = "Brand created successfully",
                Data = brand
            });
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateBrand(int id, [FromBody] BrandDto brandDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Invalid brand data",
                    Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()
                });
            }

            brandDto.Id = id;
            var brand = await _brandService.UpdateBrandAsync(id, brandDto);
            
            if (brand == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Brand not found"
                });
            }

            return Ok(new ApiResponse<BrandDto>
            {
                Success = true,
                Message = "Brand updated successfully",
                Data = brand
            });
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteBrand(int id)
        {
            var success = await _brandService.DeleteBrandAsync(id);
            
            if (!success)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Brand not found or has associated products"
                });
            }

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Brand deleted successfully"
            });
        }
    }
} 