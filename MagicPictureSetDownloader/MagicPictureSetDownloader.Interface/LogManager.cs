namespace MagicPictureSetDownloader.Interface
{
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;

    public static class LogManager
    {
        public static ILoggerFactory Factory { get; set; } = new NullLoggerFactory();
    }
}
