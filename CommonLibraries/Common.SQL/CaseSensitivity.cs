namespace Common.SQL
{
    using System;

    public class CaseSensitivity
    {
        private readonly bool _isCaseSensitive;

        public CaseSensitivity(bool isCaseSensitive)
        {
            _isCaseSensitive = isCaseSensitive;
        }

        public static string ToKeyString(string input, CaseSensitivity caseSensitivity)
        {
            if (caseSensitivity == null || caseSensitivity._isCaseSensitive || string.IsNullOrEmpty(input))
            {
                return input;
            }

            return input.ToLowerInvariant();
        }

        public static int Compare(string strA, string strB, CaseSensitivity caseSensitivity)
        {
            StringComparison stringComparison = StringComparison.Ordinal;
            if (caseSensitivity?._isCaseSensitive == false)
            {
                stringComparison = StringComparison.InvariantCultureIgnoreCase;
            }

            return string.Compare(strA, strB, stringComparison);
        }
    }
}