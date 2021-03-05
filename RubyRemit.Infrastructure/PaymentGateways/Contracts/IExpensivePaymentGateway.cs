namespace RubyRemit.Infrastructure.PaymentGateways.Contracts
{
    interface IExpensivePaymentGateway : IPaymentGateway
    {
        protected static readonly decimal minCommissionRate = 5.0M;
        protected static readonly decimal maxCommissionRate = 10.0M;
        protected static readonly decimal defaultCommissionRate = 5.0M;
    }
}
