using RubyRemit.Domain.Entities;

namespace RubyRemit.Domain.DTOs
{
    public class PaymentResponseBody
    {
        public bool Result { get; set; }


        public string Message { get; set; }


        public Payment Data { get; set; }
    }
}
