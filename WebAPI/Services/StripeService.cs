using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Stripe;
using EcommerceAPI.Services.Interfaces;

namespace EcommerceAPI.Services
{
    public class StripeService : IStripeService
    {
        private readonly IConfiguration _configuration;
        
        public StripeService(IConfiguration configuration)
        {
            _configuration = configuration;
            StripeConfiguration.ApiKey = _configuration["Stripe:SecretKey"];
        }
        
        public async Task<PaymentIntent> CreatePaymentIntentAsync(long amount, string currency = "usd")
        {
            var options = new PaymentIntentCreateOptions
            {
                Amount = amount,
                Currency = currency,
                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                {
                    Enabled = true,
                },
            };
            
            var service = new PaymentIntentService();
            return await service.CreateAsync(options);
        }
        
        public async Task<PaymentIntent> GetPaymentIntentAsync(string paymentIntentId)
        {
            var service = new PaymentIntentService();
            return await service.GetAsync(paymentIntentId);
        }
        
        public async Task<PaymentIntent> ConfirmPaymentIntentAsync(string paymentIntentId)
        {
            var service = new PaymentIntentService();
            return await service.ConfirmAsync(paymentIntentId);
        }
        
        public async Task<PaymentIntent> CancelPaymentIntentAsync(string paymentIntentId)
        {
            var service = new PaymentIntentService();
            return await service.CancelAsync(paymentIntentId);
        }
        
        public async Task<Refund> CreateRefundAsync(string paymentIntentId, long? amount = null)
        {
            var options = new RefundCreateOptions
            {
                PaymentIntent = paymentIntentId,
            };
            
            if (amount.HasValue)
            {
                options.Amount = amount.Value;
            }
            
            var service = new RefundService();
            return await service.CreateAsync(options);
        }
        
        public bool ValidateWebhookSignature(string payload, string signature, string webhookSecret)
        {
            try
            {
                var stripeEvent = EventUtility.ConstructEvent(payload, signature, webhookSecret);
                return true;
            }
            catch (StripeException)
            {
                return false;
            }
        }
    }
} 