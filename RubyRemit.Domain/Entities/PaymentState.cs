using RubyRemit.Domain.LookUp;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RubyRemit.Domain.Entities
{
    [Table("PaymentStates")]
    public class PaymentState
    {
        [Key]
        [Column("PaymentStateId")]
        public long Id { get; set; }


        [Required]
        public long PaymentId { get; set; }


        [Required]
        public PaymentStateEnum State { get; set; }


        [Required]
        public DateTime DateAttempted { get; set; }


        [Required]
        public string Gateway { get; set; }


        public string Remark { get; set; }


        public Payment Payment { get; set; }
    }
}
