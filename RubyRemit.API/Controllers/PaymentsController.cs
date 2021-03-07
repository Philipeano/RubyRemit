using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RubyRemit.Domain.DTOs;
using RubyRemit.Domain.Entities;
using RubyRemit.Domain.Interfaces;
using RubyRemit.Domain.LookUp;
using RubyRemit.Infrastructure.PaymentGateways.Contracts;
using RubyRemit.Infrastructure.Utilities;
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

        private readonly IPaymentGateway _paymentService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly Validator _validator = new Validator();

        public PaymentsController(IPaymentGateway gateway, IUnitOfWork unitOfWork)
        {
            _paymentService = gateway;
            _unitOfWork = unitOfWork;
        }


        [HttpPost]
        public ActionResult ProcessPayment([FromBody] PaymentRequestBody payment)
        {
            try
            {

                // Define rules for determining which gateway to use

                bool gatewayResult = _paymentService.ProcessTransaction(payment.CreditCardNumber, payment.CardHolder,
                    payment.ExpirationDate.ToString(), payment.SecurityCode, payment.Amount.ToString(),
                    out string message);

                if (gatewayResult == false)
                {
                    return BadRequest(JsonSerializer.Serialize(new { message }, typeof(object)));
                }
                else
                {
                    Payment newPayment = new Payment()
                    {
                        CreditCardNumber = payment.CreditCardNumber,
                        CardHolder = payment.CardHolder,
                        ExpirationDate = payment.ExpirationDate,
                        SecurityCode = payment.SecurityCode,
                        Amount = payment.Amount
                    };

                    PaymentState paymentState = new PaymentState()
                    {
                        Payment = newPayment,
                        State = PaymentStateEnum.Processed,
                        DateAttempted = DateTime.Now,
                        Gateway = _paymentService.ServiceName,
                        Remark = message
                    };

                    _unitOfWork.PaymentRepository.Add(newPayment);
                    _unitOfWork.PaymentStateRepository.Add(paymentState);
                    _unitOfWork.Commit();

                    return Ok(JsonSerializer.Serialize(new { message, data = newPayment }, typeof(object)));
                }
            }
            catch (Exception ex)
            {
                _unitOfWork.RejectChanges();
                return StatusCode(StatusCodes.Status500InternalServerError, JsonSerializer.Serialize(new { message = ex.Message }, typeof(object)));
            }
        }
    }
}
