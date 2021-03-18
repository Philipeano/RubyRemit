using RubyRemit.Domain.LookUp;

namespace RubyRemit.Infrastructure.AutoMapperSettings.Converters
{
    public class PaymentStateEnumConverter : ITypeConverter<PaymentStateEnum, string>
    {
        public string Convert(PaymentStateEnum state)
        {
            return state switch
            {
                PaymentStateEnum.Pending => "pending",
                PaymentStateEnum.Processed => "processed",
                _ => "failed",
            };
        }
    }
}