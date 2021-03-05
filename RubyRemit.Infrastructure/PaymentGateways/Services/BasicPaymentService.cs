using RubyRemit.Infrastructure.PaymentGateways.Contracts;
using System;

namespace RubyRemit.Infrastructure.PaymentGateways.Services
{
    public class BasicPaymentService : PaymentServiceBase, ICheapPaymentGateway
    {
        public BasicPaymentService(decimal? commissionRate)
        {
            serviceName = "Basic Payment Service";

            if (commissionRate == null)
            {
                activeCommissionRate = ICheapPaymentGateway.defaultCommissionRate;
                return;
            }

            if (commissionRate < ICheapPaymentGateway.minCommissionRate || commissionRate > ICheapPaymentGateway.maxCommissionRate)
                throw new ArgumentException($"For ICheapPaymentGateway, commission rate must be between {ICheapPaymentGateway.minCommissionRate}% and {ICheapPaymentGateway.maxCommissionRate}%.");
            else
                activeCommissionRate = (decimal)commissionRate;
        }
    }
}