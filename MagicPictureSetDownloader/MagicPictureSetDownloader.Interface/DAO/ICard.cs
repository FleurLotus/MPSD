namespace MagicPictureSetDownloader.Interface
{
    public interface ICard : IIdName
    {
        string Layout { get; }

        ICardFace MainCardFace { get; }
        ICardFace OtherCardFace { get; }

        string ToString(int? languageId);
        bool HasTranslation(int languageId);
        bool HasCardFace(string name);
    }
}
