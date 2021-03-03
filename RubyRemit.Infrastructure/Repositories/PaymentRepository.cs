using Microsoft.EntityFrameworkCore;
using RubyRemit.Domain.Entities;
using RubyRemit.Domain.Interfaces;
using RubyRemit.Domain.LookUp;
using RubyRemit.Infrastructure.Utilities;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace RubyRemit.Infrastructure.Repositories
{
    public class PaymentRepository : GenericRepository<Payment>, IPaymentRepository
    {
        private readonly Validator validator = new Validator();

        private IQueryable<Payment> FetchAll()
        {
            return DbSet.Include(pmt => pmt.ProcessingAttempts.OrderByDescending(pst => pst.Id));
        }


        public PaymentRepository(RubyRemitContext dbContext) : base(dbContext)
        {

        }


        public Payment AddPayment(string cardNo, string holder, string expDate, string secCode, string transAmt)
        {
            // Validate all input supplied by the user 
            if (!validator.IsValidCardNumber(cardNo, out string cardNumber, out string errorMessage))
                throw new Exception(errorMessage);

            if (!validator.IsValidHolderName(holder, out string cardHolder, out errorMessage))
                throw new Exception(errorMessage);

            if (!validator.IsValidExpirationDate(expDate, out DateTime expirationDate, out errorMessage))
                throw new Exception(errorMessage);

            if (!validator.IsValidSecurityCode(secCode, out string securityCode, out errorMessage))
                throw new Exception(errorMessage);

            if (!validator.IsValidAmount(transAmt, out decimal amount, out errorMessage))
                throw new Exception(errorMessage);

            // Validate passed. Create new payment record
            try
            {
                Payment newPayment = new Payment()
                {
                    CreditCardNumber = cardNumber,
                    CardHolder = cardHolder,
                    ExpirationDate = expirationDate,
                    SecurityCode = securityCode,
                    Amount = amount
                };

                Add(newPayment);
                return newPayment;
            }
            catch (Exception ex)
            {
                throw new Exception($"Unable to initiate payment. {ex.Message}");
            }
        }


        public PaymentStateEnum GetCurrentStatus(long paymentId)
        {
            return GetPaymentById(paymentId).ProcessingAttempts.First().State;
        }


        public int GetNumberOfAttempts(long paymentId)
        {
            return GetPaymentById(paymentId).ProcessingAttempts.Count();
        }


        public IQueryable<Payment> GetAllPayments(Expression<Func<Payment, bool>> expression)
        {
            return FetchAll().Where(expression).OrderByDescending(pmt => pmt.Id);
        }


        public Payment GetPaymentById(long id)
        {
            return FetchAll().First(pmt => pmt.Id == id);
        }
    }
}
