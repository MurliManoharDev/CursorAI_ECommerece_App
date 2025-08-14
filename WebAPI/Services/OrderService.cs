using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using AutoMapper;
using Stripe;
using EcommerceAPI.Data;
using EcommerceAPI.DTOs;
using EcommerceAPI.Models;
using EcommerceAPI.Services.Interfaces;

namespace EcommerceAPI.Services
{
    public class OrderService : IOrderService
    {
        private readonly EcommerceDbContext _context;
        private readonly IMapper _mapper;
        private readonly IStripeService _stripeService;
        private readonly ICartService _cartService;
        private readonly IConfiguration _configuration;
        
        public OrderService(
            EcommerceDbContext context, 
            IMapper mapper, 
            IStripeService stripeService,
            ICartService cartService,
            IConfiguration configuration)
        {
            _context = context;
            _mapper = mapper;
            _stripeService = stripeService;
            _cartService = cartService;
            _configuration = configuration;
        }
        
        public async Task<OrderDto> CreateOrderAsync(CreateOrderDto createOrderDto, int? userId = null)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            
            try
            {
                // Validate cart items from DTO
                if (!createOrderDto.CartItems.Any())
                {
                    throw new InvalidOperationException("Cart is empty");
                }
                
                // Validate that all products exist
                var productIds = createOrderDto.CartItems.Select(ci => ci.ProductId).Distinct().ToList();
                var existingProducts = await _context.Products
                    .Where(p => productIds.Contains(p.Id))
                    .Select(p => p.Id)
                    .ToListAsync();
                
                var missingProducts = productIds.Except(existingProducts).ToList();
                if (missingProducts.Any())
                {
                    throw new InvalidOperationException($"Products with IDs {string.Join(", ", missingProducts)} do not exist");
                }
                
                // Calculate totals
                decimal subtotal = createOrderDto.CartItems.Sum(item => item.Price * item.Quantity);
                decimal shippingCost = createOrderDto.CartItems.Sum(item => item.ShippingCost ?? 0);
                decimal tax = subtotal * 0.08m; // 8% tax rate
                decimal total = subtotal + shippingCost + tax;
                
                // Create order
                var order = new Order
                {
                    OrderNumber = GenerateOrderNumber(),
                    UserId = userId,
                    FirstName = createOrderDto.FirstName,
                    LastName = createOrderDto.LastName,
                    CompanyName = createOrderDto.CompanyName,
                    Email = createOrderDto.Email,
                    PhoneNumber = createOrderDto.PhoneNumber,
                    StreetAddress = createOrderDto.StreetAddress,
                    ApartmentSuite = createOrderDto.ApartmentSuite,
                    City = createOrderDto.City,
                    State = createOrderDto.State,
                    Country = createOrderDto.Country,
                    ZipCode = createOrderDto.ZipCode,
                    Subtotal = subtotal,
                    ShippingCost = shippingCost,
                    Tax = tax,
                    Total = total,
                    OrderNotes = createOrderDto.OrderNotes,
                    PaymentMethod = createOrderDto.PaymentMethod,
                    PaymentIntentId = createOrderDto.PaymentIntentId,
                    PaymentStatus = PaymentStatus.Pending,
                    Status = OrderStatus.Pending,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                
                // Add order items
                foreach (var cartItem in createOrderDto.CartItems)
                {
                    var orderItem = new OrderItem
                    {
                        ProductId = cartItem.ProductId,
                        ProductName = cartItem.ProductName,
                        ProductImage = cartItem.ProductImage,
                        Quantity = cartItem.Quantity,
                        Price = cartItem.Price,
                        Total = cartItem.Price * cartItem.Quantity,
                        ShippingType = cartItem.Shipping,
                        ShippingCost = cartItem.ShippingCost ?? 0
                    };
                    order.OrderItems.Add(orderItem);
                }
                
                // Add initial status history
                order.StatusHistory.Add(new OrderStatusHistory
                {
                    Status = OrderStatus.Pending.ToString(),
                    Notes = "Order created",
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow
                });
                
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();
                
                // Clear cart if user is logged in
                if (userId.HasValue)
                {
                    await _cartService.ClearCartAsync(userId.Value);
                }
                
                await transaction.CommitAsync();
                
                return _mapper.Map<OrderDto>(order);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        
        public async Task<OrderDto?> GetOrderByIdAsync(int orderId, int? userId = null)
        {
            var query = _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.StatusHistory)
                    .ThenInclude(sh => sh.User)
                .AsQueryable();
                
            if (userId.HasValue)
            {
                query = query.Where(o => o.UserId == userId.Value);
            }
            
            var order = await query.FirstOrDefaultAsync(o => o.Id == orderId);
            
            return order != null ? _mapper.Map<OrderDto>(order) : null;
        }
        
        public async Task<OrderDto?> GetOrderByOrderNumberAsync(string orderNumber)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.StatusHistory)
                    .ThenInclude(sh => sh.User)
                .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber);
                
            return order != null ? _mapper.Map<OrderDto>(order) : null;
        }
        
        public async Task<List<OrderListDto>> GetUserOrdersAsync(int userId)
        {
            var orders = await _context.Orders
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .Select(o => new OrderListDto
                {
                    Id = o.Id,
                    OrderNumber = o.OrderNumber,
                    Total = o.Total,
                    Status = o.Status.ToString(),
                    PaymentStatus = o.PaymentStatus.ToString(),
                    CreatedAt = o.CreatedAt,
                    ItemCount = o.OrderItems.Count
                })
                .ToListAsync();
                
            return orders;
        }
        
        public async Task<List<OrderDto>> GetAllOrdersAsync()
        {
            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.StatusHistory)
                    .ThenInclude(sh => sh.User)
                .Include(o => o.User)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
                
            return _mapper.Map<List<OrderDto>>(orders);
        }
        
        public async Task<OrderDto?> UpdateOrderStatusAsync(int orderId, UpdateOrderStatusDto updateStatusDto, int? userId = null)
        {
            var order = await _context.Orders
                .Include(o => o.StatusHistory)
                .FirstOrDefaultAsync(o => o.Id == orderId);
                
            if (order == null)
                return null;
                
            if (!Enum.TryParse<OrderStatus>(updateStatusDto.Status, out var newStatus))
                throw new ArgumentException("Invalid order status");
                
            order.Status = newStatus;
            order.UpdatedAt = DateTime.UtcNow;
            
            // Update specific dates based on status
            switch (newStatus)
            {
                case OrderStatus.Shipped:
                    order.ShippedAt = DateTime.UtcNow;
                    break;
                case OrderStatus.Delivered:
                    order.DeliveredAt = DateTime.UtcNow;
                    break;
            }
            
            // Add status history
            order.StatusHistory.Add(new OrderStatusHistory
            {
                Status = newStatus.ToString(),
                Notes = updateStatusDto.Notes,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            });
            
            await _context.SaveChangesAsync();
            
            return await GetOrderByIdAsync(orderId, userId);
        }
        
        public async Task<bool> CancelOrderAsync(int orderId, int? userId = null)
        {
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == orderId && (userId == null || o.UserId == userId));
                
            if (order == null || order.Status != OrderStatus.Pending)
                return false;
                
            order.Status = OrderStatus.Cancelled;
            order.UpdatedAt = DateTime.UtcNow;
            
            // Cancel payment if exists
            if (!string.IsNullOrEmpty(order.PaymentIntentId))
            {
                try
                {
                    await _stripeService.CancelPaymentIntentAsync(order.PaymentIntentId);
                    order.PaymentStatus = PaymentStatus.Cancelled;
                }
                catch
                {
                    // Log error but don't fail the cancellation
                }
            }
            
            // Add status history
            order.StatusHistory.Add(new OrderStatusHistory
            {
                Status = OrderStatus.Cancelled.ToString(),
                Notes = "Order cancelled by user",
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            });
            
            await _context.SaveChangesAsync();
            return true;
        }
        
        public async Task<StripePaymentIntentDto> CreatePaymentIntentAsync(decimal amount)
        {
            // Convert to cents for Stripe
            var amountInCents = (long)(amount * 100);
            
            var paymentIntent = await _stripeService.CreatePaymentIntentAsync(amountInCents);
            
            return new StripePaymentIntentDto
            {
                ClientSecret = paymentIntent.ClientSecret,
                PaymentIntentId = paymentIntent.Id,
                Amount = amount
            };
        }
        
        public async Task<bool> ConfirmPaymentAsync(string paymentIntentId, int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
                return false;
                
            var paymentIntent = await _stripeService.GetPaymentIntentAsync(paymentIntentId);
            
            if (paymentIntent.Status == "succeeded")
            {
                order.PaymentStatus = PaymentStatus.Succeeded;
                order.StripePaymentId = paymentIntent.Id;
                order.Status = OrderStatus.Processing;
                order.UpdatedAt = DateTime.UtcNow;
                
                // Add status history
                order.StatusHistory.Add(new OrderStatusHistory
                {
                    Status = OrderStatus.Processing.ToString(),
                    Notes = "Payment confirmed",
                    CreatedAt = DateTime.UtcNow
                });
                
                await _context.SaveChangesAsync();
                return true;
            }
            
            return false;
        }
        
        public async Task<bool> ProcessWebhookAsync(string payload, string signature)
        {
            var webhookSecret = _configuration["Stripe:WebhookSecret"];
            
            if (!_stripeService.ValidateWebhookSignature(payload, signature, webhookSecret))
                return false;
                
            var stripeEvent = EventUtility.ConstructEvent(payload, signature, webhookSecret);
            
            switch (stripeEvent.Type)
            {
                case "payment_intent.succeeded":
                    var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                    if (paymentIntent != null)
                    {
                        var order = await _context.Orders
                            .FirstOrDefaultAsync(o => o.PaymentIntentId == paymentIntent.Id);
                            
                        if (order != null)
                        {
                            order.PaymentStatus = PaymentStatus.Succeeded;
                            order.StripePaymentId = paymentIntent.Id;
                            order.Status = OrderStatus.Processing;
                            order.UpdatedAt = DateTime.UtcNow;
                            
                            await _context.SaveChangesAsync();
                        }
                    }
                    break;
                    
                case "payment_intent.payment_failed":
                    var failedPayment = stripeEvent.Data.Object as PaymentIntent;
                    if (failedPayment != null)
                    {
                        var order = await _context.Orders
                            .FirstOrDefaultAsync(o => o.PaymentIntentId == failedPayment.Id);
                            
                        if (order != null)
                        {
                            order.PaymentStatus = PaymentStatus.Failed;
                            order.UpdatedAt = DateTime.UtcNow;
                            
                            await _context.SaveChangesAsync();
                        }
                    }
                    break;
            }
            
            return true;
        }
        
        private string GenerateOrderNumber()
        {
            var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var random = new Random().Next(1000, 9999);
            return $"ORD-{timestamp}-{random}";
        }
    }
} 