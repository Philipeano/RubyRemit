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
        /// Submit a new payment request for processing with an external payment gateway, based on the details in 'paymentRequest'. 
        /// </summary>
        /// <param name="paymentRequest">A JSON object containing 'creditCardNumber', 'cardHolder', 'expirationDate', 'securityCode' and 'amount' properties</param>
        /// <returns>A JSON object containing a string 'message' property.</returns>
        /// <response code="200">Success! Payment was processed successfully.</response> 
        /// <response code="400">Bad request! Invalid input detected. See details in response body.</response>
        /// <response code="500">Error! Unable to process the payment at this time. See details in response body.</response>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(object))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(object))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(object))]
        [HttpPost("processpayment")]
        public async Task<ActionResult> ProcessPaymentAsync([FromBody] PaymentRequestBody paymentRequest)
        {
            try
            {
                bool validationResult = _orchestrator.ValidateUserInput(paymentRequest, out string validationMessage);
                if (!validationResult)
                {
                    return BadRequest(JsonSerializer.Serialize(new { message = validationMessage }, typeof(object)));
                }

                bool processingResult = await _orchestrator.ConsumePaymentService();
                if (!processingResult)
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
