namespace PaymentGateway.Validators
{
    public class LuhnValidator
    {
        public bool IsValid(string ccNumber)
        {
            var doAlternate = false;
            var sum = 0;

            ccNumber = this.RemoveWhitespace(ccNumber);

            if (ccNumber.Length < 13
                || ccNumber.Length > 19)
            {
                return false;
            }

            for (int i = ccNumber.Length - 1; i > -1; i--)
            {
                var a = ccNumber[i] - '0';
                Console.WriteLine(a);
                if (!int.TryParse(ccNumber[i].ToString(), out var mod))
                {
                    return false; // just in case the cc number has something else than a digit
                }

                if (doAlternate)
                {
                    mod *= 2;
                    if (mod > 9)
                    {
                        mod = mod % 10 + 1;
                    }
                }

                doAlternate = !doAlternate;
                sum += mod;
            }

            return sum % 10 == 0;
        }

        private string RemoveWhitespace(string input)
        {
            if(string.IsNullOrWhiteSpace(input))
            {
                return string.Empty;
            }

            return new string(input.ToCharArray()
                .Where(c => !Char.IsWhiteSpace(c))
                .ToArray());
        }
    }
}
