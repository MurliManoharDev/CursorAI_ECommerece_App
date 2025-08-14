using Microsoft.AspNetCore.Mvc;
using EcommerceAPI.DTOs;
using EcommerceAPI.Services.Interfaces;

namespace EcommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FrequentlyBoughtTogetherController : ControllerBase
    {
        private readonly IFrequentlyBoughtTogetherService _service;

        public FrequentlyBoughtTogetherController(IFrequentlyBoughtTogetherService service)
        {
            _service = service;
        }

        [HttpGet("test")]
        public async Task<ActionResult> TestConnection()
        {
            try
            {
                var count = await _service.GetCountAsync();
                var hasProduct24 = await _service.ProductExistsAsync(24);
                var relatedCount = await _service.GetRelatedCountForProductAsync(24);
                
                return Ok(new 
                { 
                    totalRecords = count,
                    product24Exists = hasProduct24,
                    product24RelatedCount = relatedCount,
                    message = "Connection successful"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message, stackTrace = ex.StackTrace });
            }
        }

        [HttpGet("product/{productId}")]
        public async Task<ActionResult<FrequentlyBoughtTogetherResponseDto>> GetFrequentlyBoughtTogether(int productId)
        {
            var result = await _service.GetFrequentlyBoughtTogetherAsync(productId);
            return Ok(result);
        }

        [HttpGet("list/{productId}")]
        public async Task<ActionResult<IEnumerable<FrequentlyBoughtTogetherDto>>> GetByProductId(int productId)
        {
            var result = await _service.GetByProductIdAsync(productId);
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<FrequentlyBoughtTogetherDto>> Create([FromBody] CreateFrequentlyBoughtTogetherDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetByProductId), new { productId = dto.ProductId }, result);
        }

        [HttpPut("{productId}/order")]
        public async Task<ActionResult> UpdateOrder(int productId, [FromBody] List<int> relatedProductIds)
        {
            var result = await _service.UpdateOrderAsync(productId, relatedProductIds);
            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpPost("seed/{productId}")]
        public async Task<ActionResult> SeedForProduct(int productId)
        {
            // Check if product exists
            var productExists = await _service.ProductExistsAsync(productId);
            if (!productExists)
                return NotFound($"Product {productId} not found");

            // Create sample frequently bought together data
            var relatedProductIds = await _service.GetRandomProductIdsAsync(productId, 3);
            if (!relatedProductIds.Any())
                return BadRequest("No other products available to create relationships");

            var dto = new CreateFrequentlyBoughtTogetherDto
            {
                ProductId = productId,
                RelatedProductIds = relatedProductIds.ToArray()
            };

            await _service.CreateAsync(dto);
            return Ok($"Created frequently bought together relationships for product {productId}");
        }

        [HttpPost("seed-all")]
        public async Task<ActionResult> SeedAll()
        {
            try
            {
                // Seed data for multiple products including product 24
                var seedData = new[]
                {
                    new { ProductId = 24, RelatedIds = new[] { 21, 25, 27 } },
                    new { ProductId = 1, RelatedIds = new[] { 2, 3, 4 } },
                    new { ProductId = 2, RelatedIds = new[] { 1, 4, 5 } },
                    new { ProductId = 3, RelatedIds = new[] { 1, 5, 6 } },
                    new { ProductId = 4, RelatedIds = new[] { 2, 5, 7 } },
                    new { ProductId = 5, RelatedIds = new[] { 3, 4, 8 } }
                };

                int created = 0;
                foreach (var item in seedData)
                {
                    var exists = await _service.GetRelatedCountForProductAsync(item.ProductId);
                    if (exists == 0)
                    {
                        var dto = new CreateFrequentlyBoughtTogetherDto
                        {
                            ProductId = item.ProductId,
                            RelatedProductIds = item.RelatedIds
                        };
                        await _service.CreateAsync(dto);
                        created++;
                    }
                }

                return Ok(new { message = $"Seeded frequently bought together data for {created} products" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
} 