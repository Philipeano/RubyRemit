using RubyRemit.Business.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace RubyRemit.Business.Services
{
    public class Validator : IValidator
    {
        private bool IsBlank(dynamic input)
        {
            if (input == null)
                return true;
            else if (input.GetType() == typeof(string) && input.Trim() == string.Empty)
                return true;
            else if (input.ToString().Trim() == string.Empty)
                return true;
            else return false;
        }


        public bool IsValidCardNumber(string input, out string output, out string errorMsg)
        {
            output = null;
            errorMsg = string.Empty;

            // Check for blank input
            if (IsBlank(input))
            {
                errorMsg = "Credit Card Number is required.";
                return false;
            }

            // Remove separators then check length of remaining characters
            string tempCardNum = input.Replace(" ", "").Replace("-", "");
            if ((tempCardNum.Length >= 7 && tempCardNum.Length <= 19) == false)
            {
                errorMsg = "Credit Card Number must have a minimum of 7 and a maximum of 19 digits.";
                return false;
            }

            // Check if remaining characters are all numeric
            char[] characters = tempCardNum.ToCharArray();
            foreach (var chr in characters)
            {
                if (int.TryParse((chr.ToString()), out _) == false)
                {
                    errorMsg = "Credit Card Number cannot contain non-numeric characters.";
                    return false;
                }
            }

            output = tempCardNum;
            return true;
        }


        public bool IsValidHolderName(string input, out string output, out string errorMsg)
        {
            output = null;
            errorMsg = string.Empty;

            // Check for blank input
            if (IsBlank(input))
            {
                errorMsg = "Cardholder Name is required.";
                return false;
            }

            // Check if length is between 2 and 25 characters
            string tempHolder = input.Trim();
            if ((tempHolder.Length >= 2 && tempHolder.Length <= 25) == false)
            {
                errorMsg = "Cardholder Name must be between 2 and 25 characters long.";
                return false;
            }

            output = tempHolder;
            return true;
        }


        public bool IsValidExpirationDate(string input, out DateTime output, out string errorMsg)
        {
            output = DateTime.MinValue;
            errorMsg = string.Empty;

            // Check for blank input
            if (IsBlank(input))
            {
                errorMsg = "Card Expiration Date is required.";
                return false;
            }

            // Check if input is a valid date
            if (DateTime.TryParse((input.ToString()), out DateTime tempExpDate) == false)
            {
                errorMsg = "Card Expiration Date must be a valid date.";
                return false;
            }

            // Check if input is a future date
            if (tempExpDate <= DateTime.Today)
            {
                errorMsg = "Card Expiration Date must be in the future.";
                return false;
            }

            output = tempExpDate;
            return true;
        }


        public bool IsValidSecurityCode(string input, out string output, out string errorMsg)
        {
            output = null;
            errorMsg = string.Empty;

            string tempSecCode = input.Trim();

            if (!IsBlank(input))
            {
                // Check if all characters are numeric
                char[] characters = tempSecCode.ToCharArray();
                foreach (var chr in characters)
                {
                    if (int.TryParse((chr.ToString()), out _) == false)
                    {
                        errorMsg = "Security Code cannot contain non-numeric characters.";
                        return false;
                    }
                }

                // Check if length is exactly 3
                if (tempSecCode.Length != 3)
                {
                    errorMsg = "Security Code must contain exactly 3 digits.";
                    return false;
                }
            }

            output = tempSecCode;
            return true;
        }


        public bool IsValidAmount(string input, out decimal output, out string errorMsg)
        {
            output = decimal.Zero;
            errorMsg = string.Empty;

            // Check for blank input
            if (IsBlank(input))
            {
                errorMsg = "Amount is required.";
                return false;
            }

            // Check if input is numeric
            if (decimal.TryParse(input, out decimal tempAmount) == false)
            {
                errorMsg = "Amount must be a decimal number.";
                return false;
            }

            // Check if input is positive
            if (tempAmount <= 0)
            {
                errorMsg = "Amount must be a positive value.";
                return false;
            }

            output = tempAmount;
            return true;
        }
    }
}
