using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RubyRemit.Business.Contracts;
using RubyRemit.Domain.DTOs;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace RubyRemit.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly IOrchestrator _orchestrator;


        public PaymentsController(IOrchestrator orchestrator)
        {
            _orchestrator = orchestrator;
        }


        // POST: api/payments/processpayment
        /// <summary>
        /// Submits a new payment request for processing with an external payment gateway, based on the details in <c>paymentRequest</c>. 
        /// </summary>
        /// <param name="paymentRequest">A JSON object containing payment details.</param>
        /// <remarks>
        /// The properties of the <c>request</c> parameter are described below: 
        /// <list type="bullet">
        /// <item><term>creditCardNumber</term> The credit card number for this transaction</item>
        /// <item><term>cardHolder</term> The customer name shown on the credit card</item>
        /// <item><term>expirationDate</term> The expiration date on the credit card</item>
        /// <item><term>securityCode</term> The 3-digit security code on the credit card, if available</item>
        /// <item><term>amount</term> The amount to be processed, in British pounds</item>
        /// </list>
        /// </remarks>
        /// <returns>A JSON object containing <c>result</c> (bool), <c>message</c> (string) and <c>data</c> (object) properties.</returns>
        /// <response code="200">Success! Payment was processed successfully.</response> 
        /// <response code="400">Bad request! Invalid input detected. See details in response body.</response>
        /// <response code="500">Error! Unable to process the payment at this time. See details in response body.</response>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(object))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(object))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(object))]
        [HttpPost("processpayment")]
        //public ActionResult<PaymentResponseBody> ProcessPaymentAsync([FromBody] PaymentRequestBody paymentRequest)
        public async Task<ActionResult<PaymentResponseBody>> ProcessPaymentAsync([FromBody] PaymentRequestBody paymentRequest)
        {
            try
            {
                bool validationResult = _orchestrator.ValidateUserInput(paymentRequest, out string validationMessage);
                if (!validationResult)
                {
                    return BadRequest(JsonSerializer.Serialize(new { message = validationMessage }, typeof(object)));
                }

                GatewayResponse processingResult = await _orchestrator.ConsumePaymentService();
                if (!processingResult.Succeeded)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, 
                        JsonSerializer.Serialize(new { message = "Unable to process the payment at this time. Please try again." }, 
                        typeof(object)));
                }
                return StatusCode(StatusCodes.Status200OK, JsonSerializer.Serialize(new { message = "Payment processed successfully." }, typeof(object)));                
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, JsonSerializer.Serialize(new { message = ex.Message }, typeof(object)));
            }
        }
    }
}
