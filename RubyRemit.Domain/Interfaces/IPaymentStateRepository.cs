using RubyRemit.Domain.Entities;
using RubyRemit.Domain.LookUp;

namespace RubyRemit.Domain.Interfaces
{
    public interface IPaymentStateRepository : IGenericRepository<PaymentState>
    {
        public PaymentState AddPaymentState(Payment payment, PaymentStateEnum state, string gateway, string remark);

    }
}
