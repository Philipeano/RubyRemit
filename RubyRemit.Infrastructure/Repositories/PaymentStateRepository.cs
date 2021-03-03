using RubyRemit.Domain.Entities;
using RubyRemit.Domain.Interfaces;
using RubyRemit.Domain.LookUp;
using System;

namespace RubyRemit.Infrastructure.Repositories
{
    public class PaymentStateRepository : GenericRepository<PaymentState>, IPaymentStateRepository
    {
        public PaymentStateRepository(RubyRemitContext dbContext) : base(dbContext)
        {

        }

        public PaymentState AddPaymentState(Payment payment, PaymentStateEnum state, string gateway, string remark)
        {
            // Create new payment state record
            try
            {
                PaymentState newPaymentState = new PaymentState()
                {
                    Payment = payment,
                    State = state,
                    DateAttempted = DateTime.Now,
                    Gateway = gateway,
                    Remark = remark
                };

                Add(newPaymentState);
                return newPaymentState;
            }
            catch (Exception ex)
            {
                throw new Exception($"Unable to update payment state. {ex.Message}");
            }
        }
    }
}
