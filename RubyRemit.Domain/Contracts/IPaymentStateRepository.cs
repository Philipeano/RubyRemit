using RubyRemit.Domain.Entities;
using RubyRemit.Domain.Interfaces;
using RubyRemit.Domain.LookUp;
using System;

namespace RubyRemit.Domain.Contracts
{
    public interface IPaymentStateRepository : IRepository<PaymentState>
    {
        PaymentState AddPaymentAttempt(long paymentId, PaymentStateEnum state, DateTime dateAttempted, 
                                       string gateway, string remark);

    }
}
