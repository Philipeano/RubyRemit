using RubyRemit.Domain.DTOs;
using System.Threading.Tasks;

namespace RubyRemit.Business.Contracts
{
    public interface IOrchestrator
    {
        public bool ValidateUserInput(PaymentRequestBody paymentInfo, out string validationMessage);


        public void ConfigureProcessingRules(decimal amount);


        public Task<bool> ConsumePaymentService();
    }
}
