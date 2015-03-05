using System;

namespace Compression.Transformation
{
    public class WrongFormattedInputException: ApplicationException
    {
        public WrongFormattedInputException(string message)
            : base(message)
        {
        }
    }
}
