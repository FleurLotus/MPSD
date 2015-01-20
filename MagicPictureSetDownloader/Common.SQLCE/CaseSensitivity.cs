namespace Common.SQLCE
{
    internal class CaseSensitivity
    {
        private readonly bool _isCaseSensitive;

        public CaseSensitivity(bool isCaseSensitive)
        {
            _isCaseSensitive = isCaseSensitive;
        }

        public string ToKeyString(string input)
        {
            if (_isCaseSensitive || string.IsNullOrEmpty(input))
                return input;

            return input.ToLowerInvariant();
        }
    }
}
