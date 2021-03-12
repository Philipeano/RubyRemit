using RubyRemit.Domain.DTOs;
using System.Threading.Tasks;

namespace RubyRemit.Business.Contracts
{
    public interface IOrchestrator
    {
        public bool ValidateUserInput(RequestBody paymentInfo, out string validationMessage);


        public void ConfigureProcessingRules(decimal amount);


        public Task<ResponseBody> ConsumePaymentService();
    }
}
