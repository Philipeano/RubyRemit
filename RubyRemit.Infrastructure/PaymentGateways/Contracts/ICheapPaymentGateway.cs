namespace RubyRemit.Infrastructure.PaymentGateways.Contracts
{
    public interface ICheapPaymentGateway : IPaymentGateway
    {
        protected static readonly decimal minCommissionRate = 0.5M;
        protected static readonly decimal maxCommissionRate = 5.0M;
        protected static readonly decimal defaultCommissionRate = 2.5M;
    }
}