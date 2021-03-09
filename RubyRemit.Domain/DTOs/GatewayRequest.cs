namespace RubyRemit.Domain.DTOs
{
    public class GatewayRequest : PaymentRequestBody
    {
        public string GatewayOption { get; set; }
    }
}
