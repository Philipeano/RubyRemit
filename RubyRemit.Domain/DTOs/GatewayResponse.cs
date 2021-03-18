namespace RubyRemit.Domain.DTOs
{
    /// <summary>
    /// Response body returned to consuming clients, with <c>succeeded</c> and <c>message</c> properties
    /// </summary>
    public class GatewayResponse
    {
        /// <summary>
        /// Boolean value indicating if the gateway successfully processed the request
        /// </summary>
        public bool Succeeded { get; set; }


        /// <summary>
        /// Brief confirmation or error message, depending on the outcome of the processing request
        /// </summary>
        public string Message { get; set; }
    }
}