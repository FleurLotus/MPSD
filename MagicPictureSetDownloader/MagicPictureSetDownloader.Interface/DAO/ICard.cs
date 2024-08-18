namespace MagicPictureSetDownloader.Interface
{
    using System;
    using System.Collections.Generic;

    public interface ICard : IIdName
    {
        IRuling[] Rulings { get; }
        IReadOnlyList<int> CardFaceIds { get; }

        string ToString(int? languageId);
        bool HasTranslation(int languageId);
        bool HasRuling(DateTime addDate, string text);
        
    }
}
