using System;
using System.ComponentModel.DataAnnotations;

namespace RubyRemit.Domain.DTOs
{
    /// <summary>
    /// Describes a payment transaction which the user intends to process.
    /// </summary>
    public class MainRequestBody
    {
        /// <summary>
        /// User's credit card number. This is a required field. 
        /// </summary>
        [Required]
        [CreditCard]
        public string CreditCardNumber { get; set; }


        /// <summary>
        /// Name of the cardholder as shown on the credit card. This is a required field.
        /// </summary>
        [Required]
        [MinLength(3, ErrorMessage = "Cardholder name must contain at least 3 letters.")]
        public string CardHolder { get; set; }


        /// <summary>
        /// Card expiration date as shown on the credit card. This is a required field.
        /// </summary>
        [Required]
        public DateTime ExpirationDate { get; set; }


        /// <summary>
        /// Security code shown on the credit card. This is an optional field.
        /// </summary>
        [RegularExpression("[0-9]{3}", ErrorMessage = "Security Code must be exactly 3 digits.")]
        public string SecurityCode { get; set; }


        /// <summary>
        /// Amount to be processed. This is a required field.
        /// </summary>
        [Required]
        [Range((double)decimal.Zero, (double)decimal.MaxValue, ErrorMessage = "Amount must be a positive number.")]
        public decimal Amount { get; set; }
    }
}