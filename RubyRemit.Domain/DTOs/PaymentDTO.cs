using System.Collections.Generic;
namespace RubyRemit.Domain.DTOs
{
    public class PaymentDTO
    {
        public long Id { get; set; }


        public string CreditCardNumber { get; set; }


        public string CardHolder { get; set; }


        public decimal Amount { get; set; }


        public virtual ICollection<PaymentStateDTO> ProcessingAttempts { get; set; }
    }
}
