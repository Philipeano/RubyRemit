using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RubyRemit.Business.Contracts;
using RubyRemit.Domain.DTOs;
using RubyRemit.Domain.Entities;
using RubyRemit.Domain.Interfaces;
using RubyRemit.Domain.LookUp;
using RubyRemit.Infrastructure.PaymentGateways.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
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


        [HttpPost]
        public async Task<ActionResult> ProcessPaymentAsync([FromBody] PaymentRequestBody payment)
        {
            try
            {
                bool validationResult = _orchestrator.ValidateUserInput(payment, out string validationMessage);
                if (!validationResult)
                {
                    return BadRequest(JsonSerializer.Serialize(new { validationMessage }, typeof(object)));
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
