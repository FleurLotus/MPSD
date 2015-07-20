namespace MagicPictureSetDownloader.Core.Upgrade
{
    using System;
    using System.Runtime.Serialization;

    public class ProgramUpgraderException : ApplicationException
    {
        public ProgramUpgraderException()
        {
        }
        public ProgramUpgraderException(string message, params object[] parameters)
            : base(string.Format(message, parameters))
        {
        }

        public ProgramUpgraderException(string message)
            : base(message)
        {
        }
        public ProgramUpgraderException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
        public ProgramUpgraderException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
