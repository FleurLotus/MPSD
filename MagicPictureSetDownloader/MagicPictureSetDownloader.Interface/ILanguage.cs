namespace MagicPictureSetDownloader.Interface
{
    public interface ILanguage : IIdName
    {
        string AlternativeName { get; }
    }

    public static class Constants
    {
        public const string English = "English";
        public const string Unknown = "Unknown";
    }
}