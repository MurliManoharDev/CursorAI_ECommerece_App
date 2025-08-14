using System.Collections.Generic;
using System.Threading.Tasks;
using EcommerceAPI.DTOs;

namespace EcommerceAPI.Services.Interfaces
{
    public interface IOrderService
    {
        Task<OrderDto> CreateOrderAsync(CreateOrderDto createOrderDto, int? userId = null);
        Task<OrderDto?> GetOrderByIdAsync(int orderId, int? userId = null);
        Task<OrderDto?> GetOrderByOrderNumberAsync(string orderNumber);
        Task<List<OrderListDto>> GetUserOrdersAsync(int userId);
        Task<List<OrderDto>> GetAllOrdersAsync();
        Task<OrderDto?> UpdateOrderStatusAsync(int orderId, UpdateOrderStatusDto updateStatusDto, int? userId = null);
        Task<bool> CancelOrderAsync(int orderId, int? userId = null);
        Task<StripePaymentIntentDto> CreatePaymentIntentAsync(decimal amount);
        Task<bool> ConfirmPaymentAsync(string paymentIntentId, int orderId);
        Task<bool> ProcessWebhookAsync(string payload, string signature);
    }
} 