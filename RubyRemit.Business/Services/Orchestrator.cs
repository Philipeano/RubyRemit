using RubyRemit.Business.Contracts;
using RubyRemit.Domain.DTOs;
using RubyRemit.Domain.Entities;
using RubyRemit.Domain.Interfaces;
using RubyRemit.Domain.LookUp;
using RubyRemit.Infrastructure.PaymentGateways.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
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

        private string _cardNumber;
        private string _holderName;
        private DateTime _expirationDate;
        private string _securityCode;
        private decimal _amount;


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


        public async Task<bool> ConsumePaymentService()
        {
            bool processingSucceeded = false;

            try
            {
                Payment newPayment = CreatePaymentRecord();

                // Attempt processing with preferred payment gateway
                _numberOfAttempts = 1;
                processingSucceeded = await _preferredGateway.ProcessTransaction(_cardNumber, _holderName, _expirationDate, _securityCode, _amount, out string message);
                newPayment.ProcessingAttempts.Add(CreatePaymentStateRecord(newPayment, processingSucceeded, _preferredGateway.ServiceName, message));

                // If processing fails at first attempt, try subsequently with backup gateway (subject to maximum allowed retries)
                while (!processingSucceeded && _numberOfAttempts < _maximumAttempts)
                {
                    _numberOfAttempts += 1;
                    processingSucceeded = await _backupGateway.ProcessTransaction(_cardNumber, _holderName, _expirationDate, _securityCode, _amount, out message);
                    newPayment.ProcessingAttempts.Add(CreatePaymentStateRecord(newPayment, processingSucceeded, _backupGateway.ServiceName, message));
                }

                // Save payment and payment state info to database
                _unitOfWork.PaymentRepository.Add(newPayment);
                _unitOfWork.Commit();
            }
            catch
            {
                _unitOfWork.RejectChanges();
            }

            return processingSucceeded;
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
