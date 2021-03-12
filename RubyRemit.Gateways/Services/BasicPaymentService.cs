using RubyRemit.Infrastructure.PaymentGateways.Contracts;

namespace RubyRemit.Gateways.Services
{
    public class BasicPaymentService : PaymentServiceBase, ICheapPaymentGateway
    {
        public BasicPaymentService()
        {
            serviceName = "Basic Payment Service";

            activeCommissionRate = ICheapPaymentGateway.defaultCommissionRate;
        }
    }
}