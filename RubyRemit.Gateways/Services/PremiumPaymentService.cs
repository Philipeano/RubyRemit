using RubyRemit.Infrastructure.PaymentGateways.Contracts;
using System;

namespace RubyRemit.Gateways.Services
{

    public class PremiumPaymentService : PaymentServiceBase, IExpensivePaymentGateway
    {
        public PremiumPaymentService(decimal? commissionRate)
        {
            serviceName = "Premium Payment Service";

            if (commissionRate == null)
            {
                activeCommissionRate = IExpensivePaymentGateway.defaultCommissionRate;
                return;
            }

            if (commissionRate < IExpensivePaymentGateway.minCommissionRate || commissionRate > IExpensivePaymentGateway.maxCommissionRate)
                throw new ArgumentException($"For IExpensivePaymentGateway, commission rate must be between {IExpensivePaymentGateway.minCommissionRate}% and {IExpensivePaymentGateway.maxCommissionRate}%.");
            else
                activeCommissionRate = (decimal)commissionRate;
        }
    }
}
