using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EcommerceAPI.DTOs;
using EcommerceAPI.Services.Interfaces;

namespace EcommerceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        
        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }
        
        [HttpPost("create")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto createOrderDto)
        {
            try
            {
                int? userId = null;
                if (User.Identity?.IsAuthenticated == true)
                {
                    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    if (int.TryParse(userIdClaim, out var parsedUserId))
                    {
                        userId = parsedUserId;
                    }
                }
                
                // For guest checkout, userId will remain null
                // The Order model already supports nullable UserId
                
                var order = await _orderService.CreateOrderAsync(createOrderDto, userId);
                return Ok(order);
            }
            catch (InvalidOperationException ex)
            {
                // Return specific error messages for known issues
                if (ex.Message.Contains("do not exist"))
                {
                    return BadRequest(new { 
                        message = "Some products in your cart are no longer available. Please refresh the page and try again.",
                        error = ex.Message 
                    });
                }
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // Log the full exception details
                var innerException = ex.InnerException?.Message ?? "No inner exception";
                var fullError = $"Error: {ex.Message}. Inner: {innerException}";
                
                return StatusCode(500, new { 
                    message = "An error occurred while creating the order", 
                    error = ex.Message,
                    innerError = innerException,
                    stackTrace = ex.StackTrace
                });
            }
        }
        
        [HttpPost("payment-intent")]
        public async Task<ActionResult<StripePaymentIntentDto>> CreatePaymentIntent([FromBody] CreatePaymentIntentDto dto)
        {
            try
            {
                var paymentIntent = await _orderService.CreatePaymentIntentAsync(dto.Amount);
                return Ok(paymentIntent);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating payment intent", error = ex.Message });
            }
        }
        
        [HttpPost("{orderId}/confirm-payment")]
        public async Task<ActionResult> ConfirmPayment(int orderId, [FromBody] ConfirmPaymentDto dto)
        {
            try
            {
                var result = await _orderService.ConfirmPaymentAsync(dto.PaymentIntentId, orderId);
                
                if (result)
                    return Ok(new { message = "Payment confirmed successfully" });
                    
                return BadRequest(new { message = "Payment confirmation failed" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while confirming payment", error = ex.Message });
            }
        }
        
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDto>> GetOrder(int id)
        {
            int? userId = null;
            if (User.Identity?.IsAuthenticated == true)
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (int.TryParse(userIdClaim, out var parsedUserId))
                {
                    userId = parsedUserId;
                }
            }
            
            var order = await _orderService.GetOrderByIdAsync(id, userId);
            
            if (order == null)
                return NotFound();
                
            return Ok(order);
        }
        
        [HttpGet("order-number/{orderNumber}")]
        public async Task<ActionResult<OrderDto>> GetOrderByNumber(string orderNumber)
        {
            var order = await _orderService.GetOrderByOrderNumberAsync(orderNumber);
            
            if (order == null)
                return NotFound();
                
            return Ok(order);
        }
        
        [Authorize]
        [HttpGet("my-orders")]
        public async Task<ActionResult<List<OrderListDto>>> GetMyOrders()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }
            
            var orders = await _orderService.GetUserOrdersAsync(userId);
            return Ok(orders);
        }
        
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<List<OrderDto>>> GetAllOrders()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return Ok(orders);
        }
        
        [Authorize]
        [HttpPut("{id}/status")]
        public async Task<ActionResult<OrderDto>> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusDto updateStatusDto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int? userId = null;
            
            if (int.TryParse(userIdClaim, out var parsedUserId))
            {
                userId = parsedUserId;
            }
            
            try
            {
                var order = await _orderService.UpdateOrderStatusAsync(id, updateStatusDto, userId);
                
                if (order == null)
                    return NotFound();
                    
                return Ok(order);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        
        [Authorize]
        [HttpPost("{id}/cancel")]
        public async Task<ActionResult> CancelOrder(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int? userId = null;
            
            if (int.TryParse(userIdClaim, out var parsedUserId))
            {
                userId = parsedUserId;
            }
            
            var result = await _orderService.CancelOrderAsync(id, userId);
            
            if (result)
                return Ok(new { message = "Order cancelled successfully" });
                
            return BadRequest(new { message = "Unable to cancel order" });
        }
        
        [HttpPost("webhook")]
        public async Task<ActionResult> StripeWebhook()
        {
            var payload = await new System.IO.StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            var signature = Request.Headers["Stripe-Signature"];
            
            try
            {
                var result = await _orderService.ProcessWebhookAsync(payload, signature);
                
                if (result)
                    return Ok();
                    
                return BadRequest();
            }
            catch
            {
                return BadRequest();
            }
        }
    }
    
    public class CreatePaymentIntentDto
    {
        public decimal Amount { get; set; }
    }
    
    public class ConfirmPaymentDto
    {
        public string PaymentIntentId { get; set; } = string.Empty;
    }
} 