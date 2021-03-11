using RubyRemit.Domain.LookUp;
using System;

namespace RubyRemit.Domain.DTOs
{
    public class PaymentStateDTO
    {
        public PaymentStateEnum State { get; set; }


        public DateTime DateAttempted { get; set; }


        public string Gateway { get; set; }


        public string Remark { get; set; }
    }
}
