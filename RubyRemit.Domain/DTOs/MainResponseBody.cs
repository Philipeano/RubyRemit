namespace RubyRemit.Domain.DTOs
{
    /// <summary>
    /// Default response body for all requests, with <c>succeeded</c>, <c>message</c> and <c>data</c> properties. 
    /// </summary>
    public class MainResponseBody
    {
        /// <summary>
        /// Boolean value indicating if the request is successful or not
        /// </summary>
        public bool Succeeded { get; set; }


        /// <summary>
        /// Brief confirmation message if the request is successful, or error message otherwise.
        /// </summary>
        public string Message { get; set; }


        /// <summary>
        /// If not null, contains the payload returned for the request.
        /// </summary>
        public dynamic Data { get; set; }
    }
}