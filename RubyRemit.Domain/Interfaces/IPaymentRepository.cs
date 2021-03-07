using RubyRemit.Domain.Entities;
using RubyRemit.Domain.LookUp;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace RubyRemit.Domain.Interfaces
{
    public interface IPaymentRepository : IGenericRepository<Payment>
    {
        public Payment AddPayment(string cardNumber, string cardHolder, DateTime expirationDate, string securityCode, decimal amount);


        public PaymentStateEnum GetCurrentStatus(long paymentId);


        public int GetNumberOfAttempts(long paymentId);


        public IQueryable<Payment> GetAllPayments(Expression<Func<Payment, bool>> expression);


        public Payment GetPaymentById(long id);
    } 
}
