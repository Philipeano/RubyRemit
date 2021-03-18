using System;

namespace RubyRemit.Domain.DTOs
{
    /// <summary>
    /// Describes a specific attempt made at processing a payment.
    /// </summary>
    public class PaymentStateDTO
    {
        /// <summary>
        /// Status of the processing attempt. The value is any of <c>pending</c>,  <c>processed</c> or  <c>failed</c>.
        /// </summary>
        public string State { get; set; }


        /// <summary>
        /// Date and time the processing was attempted
        /// </summary>
        public DateTime DateAttempted { get; set; }


        /// <summary>
        /// Category of payment gateway used. The value is either <c>cheap</c> or <c>expensive</c>.
        /// </summary>
        public string Gateway { get; set; }


        /// <summary>
        /// Transaction summary or error message, based on the response received from the payment gateway
        /// </summary>
        public string Remark { get; set; }
    }
}