using System;
using RubyRemit.Infrastructure.Utilities;

namespace RubyRemit.Infrastructure.PaymentGateways.Services
{
    public abstract class PaymentServiceBase
    {
        protected string serviceName;
        protected decimal activeCommissionRate;
        private readonly Validator validator = new Validator();


        public string ServiceName => serviceName;


        public decimal CommissionRate => activeCommissionRate;


        public decimal CalculateCommission(decimal amount, decimal commissionRate)
        {
            return commissionRate / 100 * amount;
        }


        public bool ProcessTransaction(string cardNo, string holder, string expDate, string secCode, string transAmt, out string message)
        {
            // Generate a random number between 1 and 10 to simulate checking if the service is available. Half the time, it should be available (when greater than 5)
            if (new Random().Next(1, 10) <= 5)
            {
                // Service is unavailable. Abort further processing...
                message = $"Unable to process the payment with {serviceName}. The service is currently unavailable.";
                return false;
            }

            // Validate all input supplied by the user, and abort if any input value is invalid  
            if (!validator.IsValidCardNumber(cardNo, out _, out string errorMessage))
            {
                message = $"Validation error: {errorMessage}";
                return false;
            }

            if (!validator.IsValidHolderName(holder, out _, out errorMessage))
            {
                message = $"Validation error: {errorMessage}";
                return false;
            }

            if (!validator.IsValidExpirationDate(expDate, out _, out errorMessage))
            {
                message = $"Validation error: {errorMessage}";
                return false;
            }

            if (!validator.IsValidSecurityCode(secCode, out _, out errorMessage))
            {
                message = $"Validation error: {errorMessage}";
                return false;
            }

            if (!validator.IsValidAmount(transAmt, out _, out errorMessage))
            {
                message = $"Validation error: {errorMessage}";
                return false;
            }

            // Validation passed. Attempt further processing...
            try
            {
                decimal commRate = decimal.Round(activeCommissionRate, 2);
                decimal commAmount = decimal.Round(CalculateCommission(decimal.Parse(transAmt), activeCommissionRate), 2);
                message = $"Successfully processed the payment with {serviceName}. Commission is £{commAmount} at {commRate}%";
                return true;
            }
            catch (Exception ex)
            {
                message = $"Unable to process the payment with {serviceName}. {ex.Message}";
                return false;
            }
        }
    }
}