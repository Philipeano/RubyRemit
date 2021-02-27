using RubyRemit.Domain.Entities;
using RubyRemit.Domain.Interfaces;
using System;

namespace RubyRemit.Domain.Contracts
{
    public interface IPaymentRepository : IRepository<Payment>
    {
        Payment AddPayment(string cardNumber, string cardHolder, DateTime expiryDate, string securityCode,
                           decimal amount, DateTime dateInitiated);
    }
}
