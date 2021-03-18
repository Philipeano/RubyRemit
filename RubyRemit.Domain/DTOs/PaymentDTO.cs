using System.Collections.Generic;

namespace RubyRemit.Domain.DTOs
{
    /// <summary>
    /// Summarizes the payment information supplied by the user for processing.
    /// </summary>
    public class PaymentDTO
    {
        /// <summary>
        /// Unique identifier for this payment record
        /// </summary>
        public long Id { get; set; }


        /// <summary>
        /// Credit card number entered by the user
        /// </summary>
        public string CreditCardNumber { get; set; }


        /// <summary>
        /// Cardholder name entered by the user
        /// </summary>
        public string CardHolder { get; set; }


        /// <summary>
        /// Amount specified by the user
        /// </summary>
        public decimal Amount { get; set; }


        /// <summary>
        /// List of processing attempts for this payment  
        /// </summary>
        public virtual ICollection<PaymentStateDTO> ProcessingAttempts { get; set; }
    }
}