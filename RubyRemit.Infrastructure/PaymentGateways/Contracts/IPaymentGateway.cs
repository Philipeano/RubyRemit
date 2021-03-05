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

        public bool ProcessTransaction(string cardNo, string holder, string expDate, string secCode, string transAmt, out string message);
    }
}