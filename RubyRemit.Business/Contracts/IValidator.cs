using System;

namespace RubyRemit.Business.Contracts
{
    public interface IValidator
    {
        public bool IsValidAmount(string input, out decimal output, out string errorMsg);

        public bool IsValidCardNumber(string input, out string output, out string errorMsg);

        public bool IsValidExpirationDate(string input, out DateTime output, out string errorMsg);

        public bool IsValidHolderName(string input, out string output, out string errorMsg);

        public bool IsValidSecurityCode(string input, out string output, out string errorMsg);
    }
}
