using RubyRemit.Domain.DTOs;
using System.Threading.Tasks;

namespace RubyRemit.Business.Contracts
{
    public interface IOrchestrator
    {
        public bool ValidateUserInput(MainRequestBody paymentInfo, out string validationMessage);


        public void ConfigureProcessingRules(decimal amount);


        public Task<MainResponseBody> ConsumePaymentService();
    }
}
