namespace MagicPictureSetDownloader.Interface
{
    public interface ITranslate
    {
        int IdCard { get; set; }
        int IdLanguage { get; set; }
        string Name { get; set; }
    }
}