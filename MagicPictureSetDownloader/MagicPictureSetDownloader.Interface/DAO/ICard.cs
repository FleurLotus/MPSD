namespace MagicPictureSetDownloader.Interface
{
    using System;

    public interface ICard : IIdName
    {
        string Text { get; }
        string Power { get; }
        string Toughness { get; }
        string CastingCost { get; }
        string Loyalty { get; }
        string Defense { get; }
        string Type { get; }
        string PartName { get; }
        string OtherPartName { get; }
        IRuling[] Rulings { get; }

        string ToString(int? languageId);
        bool HasTranslation(int languageId);
        bool HasRuling(DateTime addDate, string text);
    }
}
