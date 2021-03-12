using RubyRemit.Domain.DTOs;
using RubyRemit.Infrastructure.PaymentGateways.Contracts;
using System;
using System.Threading.Tasks;

namespace RubyRemit.Gateways.Services
{
    public abstract class PaymentServiceBase : IPaymentGateway
    {
        protected string serviceName;
        protected decimal activeCommissionRate;


        public string ServiceName => serviceName;


        public decimal CommissionRate => activeCommissionRate;


        public decimal CalculateCommission(decimal amount, decimal commissionRate)
        {
            return commissionRate / 100 * amount;
        }


        public Task<GatewayResponse> ProcessTransaction(MainRequestBody request)
        {
            GatewayResponse response = new GatewayResponse();

            // Generate a random number between 1 and 10 to simulate checking if the service is available. 
            // Half the time, it should be available (when greater than 5)
            if (new Random().Next(1, 10) <= 5)
            {
                // Service is unavailable. Abort further processing...
                response.Succeeded = false;
                response.Message = $"Unable to process the payment for {request.CardHolder.ToUpper()} with {serviceName}. The service is currently unavailable.";
                return Task.FromResult(response);
            }

            // Attempt further processing...
            try
            {
                decimal commRate = decimal.Round(activeCommissionRate, 2);
                decimal commAmount = decimal.Round(CalculateCommission(request.Amount, activeCommissionRate), 2);
                response.Succeeded = true;
                response.Message = $"Successfully processed payment for {request.CardHolder.ToUpper()} with {serviceName}. Commission is £{commAmount} at {commRate}%";
                return Task.FromResult(response);
            }
            catch (Exception ex)
            {
                response.Succeeded = false;
                response.Message = $"Unable to process the payment with {serviceName}. {ex.Message}";
                return Task.FromResult(response);
            }
        }
    }
}
