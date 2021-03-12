using RubyRemit.Infrastructure.PaymentGateways.Contracts;

namespace RubyRemit.Gateways.Services
{
    public class PremiumPaymentService : PaymentServiceBase, IExpensivePaymentGateway
    {
        public PremiumPaymentService()
        {
            serviceName = "Premium Payment Service";

            activeCommissionRate = IExpensivePaymentGateway.defaultCommissionRate;
        }
    }
}
