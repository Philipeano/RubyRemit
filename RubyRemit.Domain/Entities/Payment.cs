using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RubyRemit.Domain.Entities
{
    [Table("Payments")]
    public class Payment
    {
        [Key]
        [Column("PaymentId")]
        public long Id { get; set; }


        [Required]
        [CreditCard]
        public string CreditCardNumber { get; set; }


        [Required]
        [MinLength(3, ErrorMessage = "Cardholder name must contain at least 3 letters.")]
        public string CardHolder { get; set; }


        [Required]
        public DateTime ExpirationDate { get; set; }


        [RegularExpression("/[0-9]{3}/", ErrorMessage = "Security Code must be exactly 3 digits.")]
        public string SecurityCode { get; set; }


        [Required]
        [Range((double)decimal.Zero, (double)decimal.MaxValue, ErrorMessage = "Amount must be a positive number.")]
        public decimal Amount { get; set; }


        [Required]
        public DateTime DateInitiated { get; set; }


        public virtual ICollection<PaymentState> ProcessingAttempts { get; set; }
    }
}
