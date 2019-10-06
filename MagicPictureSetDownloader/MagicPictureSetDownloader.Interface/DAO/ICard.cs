﻿namespace MagicPictureSetDownloader.Interface
{
    using System;

    public interface ICard : IIdName
    {
        string Text { get; }
        string Power { get; }
        string Toughness { get; }
        string CastingCost { get; }
        string Loyalty { get; }
        string Type { get; }
        string PartName { get; }
        string OtherPartName { get; }
        IRuling[] Rulings { get; }

        bool IsMultiPart { get; }
        bool IsReverseSide { get; }
        bool IsSplitted { get; }
        bool Is90DegreeSide { get; }
        bool IsSameDisplay { get; }
        bool IsMultiCard { get; }
        string ToString(int? languageId);
        bool HasTranslation(int languageId);
        bool HasRuling(DateTime addDate, string text);
    }
}