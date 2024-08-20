namespace MagicPictureSetDownloader.Interface
{
    using System.Collections.Generic;

    public interface ICard : IIdName
    {
        IReadOnlyList<int> CardFaceIds { get; }

        string ToString(int? languageId);
        bool HasTranslation(int languageId);
    }
}
