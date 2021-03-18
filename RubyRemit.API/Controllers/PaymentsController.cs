using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RubyRemit.Business.Contracts;
using RubyRemit.Domain.DTOs;
using System;
using System.Threading.Tasks;

namespace RubyRemit.Api.Controllers
{
    [Route("api/payments")]
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
        /// The properties of the <c>paymentRequest</c> parameter are described below: 
        /// <list type="bullet">
        /// 
        /// <listheader><term>Property</term><description>Description</description></listheader>
        /// 
        /// <item><term>creditCardNumber</term> <description>The credit card number for this transaction</description></item>
        /// <item><term>cardHolder</term> <description>The customer name shown on the credit card</description></item>
        /// <item><term>expirationDate</term> <description>The expiration date on the credit card</description></item>
        /// <item><term>securityCode</term> <description>The 3-digit security code on the credit card, if available</description></item>
        /// <item><term>amount</term> <description>The amount to be processed, in British pounds</description></item>
        /// </list>
        /// </remarks>
        /// <returns>A JSON object containing a <c>succeeded</c> (bool), <c>message</c> (string) and <c>data</c> (object) properties.</returns>
        /// <response code="200">Success! Payment was processed successfully.</response> 
        /// <response code="400">Bad request! Invalid input detected. See details in response body.</response>
        /// <response code="500">Error! Unable to process the payment at this time. See details in response body.</response>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(object))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(object))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(object))]
        [HttpPost("processpayment")]
        public async Task<ActionResult<MainResponseBody>> ProcessPayment([FromBody] MainRequestBody paymentRequest)
        {
            MainResponseBody processingResult;
            try
            {
                bool validationResult = _orchestrator.ValidateUserInput(paymentRequest, out string validationMessage);
                if (!validationResult)
                {
                    processingResult = new MainResponseBody() { Succeeded = false, Message = validationMessage, Data = null };
                    return BadRequest(processingResult);
                }

                processingResult = await _orchestrator.ConsumePaymentService();
                if (!processingResult.Succeeded)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, processingResult);
                }
                return StatusCode(StatusCodes.Status200OK, processingResult);
            }
            catch (Exception ex)
            {
                processingResult = new MainResponseBody() { Succeeded = false, Message = ex.Message, Data = null };
                return StatusCode(StatusCodes.Status500InternalServerError, processingResult);
            }
        }
    }
}
