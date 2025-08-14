using System.Threading.Tasks;
using Stripe;

namespace EcommerceAPI.Services.Interfaces
{
    public interface IStripeService
    {
        Task<PaymentIntent> CreatePaymentIntentAsync(long amount, string currency = "usd");
        Task<PaymentIntent> GetPaymentIntentAsync(string paymentIntentId);
        Task<PaymentIntent> ConfirmPaymentIntentAsync(string paymentIntentId);
        Task<PaymentIntent> CancelPaymentIntentAsync(string paymentIntentId);
        Task<Refund> CreateRefundAsync(string paymentIntentId, long? amount = null);
        bool ValidateWebhookSignature(string payload, string signature, string webhookSecret);
    }
} 