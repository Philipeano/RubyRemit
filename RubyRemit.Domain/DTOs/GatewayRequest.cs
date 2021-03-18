namespace RubyRemit.Domain.DTOs
{
    /// <summary>
    /// Describes a processing request being sent by the main API to the external payment gateway.
    /// </summary>
    public class GatewayRequest : MainRequestBody
    {
        /// <summary>
        /// Specifies what category of payment gateway to use. The value is any of <c>cheap</c> or <c>expensive</c>.
        /// </summary>
        public string GatewayOption { get; set; }
    }
}