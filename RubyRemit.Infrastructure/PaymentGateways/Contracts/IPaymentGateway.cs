using RubyRemit.Domain.DTOs;
using System.Threading.Tasks;

namespace RubyRemit.Infrastructure.PaymentGateways.Contracts
{
    public interface IPaymentGateway
    {        
        public string ServiceName { get; }


        public decimal CommissionRate { get; }


        public decimal CalculateCommission(decimal amount, decimal commissionRate);


        public Task<GatewayResponse> ProcessTransaction(MainRequestBody request);
   }
}