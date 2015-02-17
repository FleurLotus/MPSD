namespace MagicPictureSetDownloader.Interface
{
    public interface ITranslate
    {
        int IdCard { get;  }
        int IdLanguage { get; }
        string Name { get; }
    }
}
