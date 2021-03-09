using RubyRemit.Business.Contracts;
using RubyRemit.Domain.DTOs;
using RubyRemit.Domain.Entities;
using RubyRemit.Domain.Interfaces;
using RubyRemit.Domain.LookUp;
using RubyRemit.Infrastructure.PaymentGateways.Contracts;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RubyRemit.Business.Services
{
    public class Orchestrator : IOrchestrator
    {
        private bool _isConfigured = false;
        private short _maximumAttempts;
        private short _numberOfAttempts;

        private readonly IPaymentGateway _cheapService, _expensiveService;
        private IPaymentGateway _preferredGateway, _backupGateway;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator _validator;

        private GatewayRequest _requestBody;
        private string _cardNumber;
        private string _holderName;
        private DateTime _expirationDate;
        private string _securityCode;
        private decimal _amount;

        private HttpClient _httpClient = new HttpClient();


        public bool IsConfigured => _isConfigured;


        public Orchestrator(ICheapPaymentGateway cheapGateway, IExpensivePaymentGateway expensiveGateway, IValidator validator, IUnitOfWork unitOfWork)
        {
            _cheapService = cheapGateway;
            _expensiveService = expensiveGateway;
            _validator = validator;
            _unitOfWork = unitOfWork;
        }


        public bool ValidateUserInput(PaymentRequestBody paymentInfo, out string validationMessage)
        {
            bool validationResult, result1, result2, result3, result4, result5;

            result1 = _validator.IsValidCardNumber(paymentInfo.CreditCardNumber, out _cardNumber, out string message1);
            result2 = _validator.IsValidHolderName(paymentInfo.CardHolder, out _holderName, out string message2);
            result3 = _validator.IsValidExpirationDate(paymentInfo.ExpirationDate.ToString(), out _expirationDate, out string message3);
            result4 = _validator.IsValidSecurityCode(paymentInfo.SecurityCode, out _securityCode, out string message4);
            result5 = _validator.IsValidAmount(paymentInfo.Amount.ToString(), out _amount, out string message5);

            validationMessage = $"{message1} {message2} {message3} {message4} {message5}";
            validationResult = result1 || result2 || result3 || result4 || result5;

            if (validationResult)
            {
                try
                {
                    _requestBody = new GatewayRequest()
                    {
                        CreditCardNumber = paymentInfo.CreditCardNumber,
                        CardHolder = paymentInfo.CardHolder,
                        ExpirationDate = paymentInfo.ExpirationDate,
                        SecurityCode = paymentInfo.SecurityCode,
                        Amount = paymentInfo.Amount
                    };

                    ConfigureProcessingRules(_amount);
                }
                catch (Exception ex)
                {
                    validationMessage = ex.Message;
                    validationResult = false;
                }
            }
            return validationResult;
        }


        public void ConfigureProcessingRules(decimal amount)
        {
            _isConfigured = true;

            if (amount <= 20)
            {
                _preferredGateway = _cheapService;
                _maximumAttempts = 1;
            }
            else if (amount >= 21 && amount <= 500)
            {
                _preferredGateway = _expensiveService;
                _backupGateway = _cheapService;
                _maximumAttempts = 2;
            }
            else if (amount > 500)
            {
                _preferredGateway = _expensiveService;
                _backupGateway = _expensiveService;
                _maximumAttempts = 4;
            }
            else
            {
                _isConfigured = false;
                throw new Exception("Unable to determine the payment gateway to use. Check the amount and try again.");
            }
        }


        public async Task<GatewayResponse> ConsumePaymentService()
        {
            GatewayResponse gatewayResponse = new GatewayResponse();

            try
            {
                Payment newPayment = CreatePaymentRecord();
                _httpClient.BaseAddress = new Uri("https://localhost:1000/"); // TODO: Extract this line into Startup.CongureServices()
                string relativePath = "api/gateways/process";

                // Attempt processing with preferred payment gateway
                _numberOfAttempts = 1;
                _requestBody.GatewayOption = _preferredGateway.ServiceName.ToLower().Contains("cheap") ? "cheap" : "expensive";
                StringContent requestBodyJson = new StringContent(JsonSerializer.Serialize(_requestBody), Encoding.UTF8, "application/json");
                var httpResponse = await _httpClient.PostAsync(relativePath, requestBodyJson);
                gatewayResponse.Succeeded = httpResponse.IsSuccessStatusCode;
                gatewayResponse.Message = httpResponse.Content.ToString();
                newPayment.ProcessingAttempts.Add(CreatePaymentStateRecord(newPayment, gatewayResponse.Succeeded, _preferredGateway.ServiceName, gatewayResponse.Message));

                // If processing fails at first attempt, try subsequently with backup gateway (subject to maximum allowed retries)
                while (httpResponse.StatusCode != HttpStatusCode.OK && _numberOfAttempts < _maximumAttempts)
                // while (!gatewayResponse.Succeeded && _numberOfAttempts < _maximumAttempts)
                {
                    _numberOfAttempts += 1;
                    _requestBody.GatewayOption = _backupGateway.ServiceName.ToLower().Contains("cheap") || _backupGateway.ServiceName.ToLower().Contains("basic") ? "cheap" : "expensive";
                    requestBodyJson = new StringContent(JsonSerializer.Serialize(_requestBody), Encoding.UTF8, "application/json");
                    httpResponse = await _httpClient.PostAsync(relativePath, requestBodyJson);
                    gatewayResponse.Succeeded = httpResponse.IsSuccessStatusCode;
                    gatewayResponse.Message = httpResponse.Content.ToString();
                    newPayment.ProcessingAttempts.Add(CreatePaymentStateRecord(newPayment, gatewayResponse.Succeeded, _preferredGateway.ServiceName, gatewayResponse.Message));
                }

                // Save payment and payment state info to database
                _unitOfWork.PaymentRepository.Add(newPayment);
                _unitOfWork.Commit();
                return gatewayResponse;
            }
            catch (Exception ex)
            {
                // Discard all entity changes and report exception
                _unitOfWork.RejectChanges();
                gatewayResponse = new GatewayResponse() { Succeeded = false, Message = ex.Message };
                return gatewayResponse;
            }
        }


        private Payment CreatePaymentRecord()
        {
            return new Payment
            {
                CreditCardNumber = _cardNumber,
                CardHolder = _holderName,
                ExpirationDate = _expirationDate,
                SecurityCode = _securityCode,
                Amount = _amount
            };
        }


        private PaymentState CreatePaymentStateRecord(Payment payment, bool result, string serviceName, string remark)
        {
            return new PaymentState
            {
                Payment = payment,
                DateAttempted = DateTime.Now,
                Gateway = serviceName,
                State = result ? PaymentStateEnum.Processed : PaymentStateEnum.Failed,
                Remark = remark
            };
        }
    }
}
