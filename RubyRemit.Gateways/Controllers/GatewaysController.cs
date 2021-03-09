using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RubyRemit.Domain.DTOs;
using RubyRemit.Infrastructure.PaymentGateways.Contracts;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace RubyRemit.Gateways.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GatewaysController : ControllerBase
    {

        private IPaymentGateway _selectedService;
        private readonly IPaymentGateway _cheapService, _expensiveService;


        public GatewaysController(ICheapPaymentGateway cheapGateway, IExpensivePaymentGateway expensiveGateway)
        {
            _cheapService = cheapGateway;
            _expensiveService = expensiveGateway;
        }


        // POST: api/gateways/process
        /// <summary>
        /// Receives a payment processing request from a 3rd-party and invokes one of the gateway implementations to handle processing. 
        /// </summary>
        /// <param name="request">A JSON object containing payment details.</param>
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
        /// <returns>A JSON object containing <c>success</c> (boolean) and <c>message</c> (string) properties.</returns>
        /// <response code="200">Success! Processing was completed successfully.</response> 
        /// <response code="500">Error! Processing failed due to service unavailability or other error.</response>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(object))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(object))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(object))]
        [HttpPost("process")]
        public async Task<ActionResult<GatewayResponse>> ProcessAsync([FromBody] GatewayRequest request)
        {
            GatewayResponse gatewayResponse;

            switch (request.GatewayOption.ToLower())
            {
                case "cheap":
                case "basic":
                    _selectedService = _cheapService;
                    break;
                case "expensive":
                case "premium":
                    _selectedService = _expensiveService;
                    break;
                default:
                    gatewayResponse = new GatewayResponse() { Succeeded = false, Message = "Missing or invalid payment gateway option." };
                    return StatusCode(StatusCodes.Status400BadRequest, JsonSerializer.Serialize(gatewayResponse, typeof(GatewayResponse)));
            }


            try
            {
                gatewayResponse = await _selectedService.ProcessTransaction(request);
                if (!gatewayResponse.Succeeded)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, JsonSerializer.Serialize(gatewayResponse, typeof(GatewayResponse)));
                }
                else
                {
                    return StatusCode(StatusCodes.Status200OK, JsonSerializer.Serialize(gatewayResponse, typeof(GatewayResponse)));
                }
            }
            catch (Exception ex)
            {
                gatewayResponse = new GatewayResponse(){ Succeeded = false, Message = ex.Message };
                return StatusCode(StatusCodes.Status500InternalServerError, JsonSerializer.Serialize(gatewayResponse, typeof(GatewayResponse)));
            }
        }
    }
}
