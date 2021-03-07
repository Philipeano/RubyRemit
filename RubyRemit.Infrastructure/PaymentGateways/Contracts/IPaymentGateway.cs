using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RubyRemit.Infrastructure.PaymentGateways.Contracts
{
    public interface IPaymentGateway
    {        
        public string ServiceName { get; }

        public decimal CommissionRate { get; }

        public decimal CalculateCommission(decimal amount, decimal commissionRate);

        public Task<bool> ProcessTransaction(string cardNo, string holder, DateTime expDate, string secCode, decimal transAmt, out string message);
    }
}