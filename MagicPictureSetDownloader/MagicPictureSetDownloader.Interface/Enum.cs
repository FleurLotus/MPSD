using System;

namespace MagicPictureSetDownloader.Interface
{
    public enum TypeOfOption
    {
        Hierarchy,
        SelectedCollection,
        Display,
        Input,
        Upgrade,
    }

    public enum ExportFormat
    {
        MPSD = -1,
        MTGM = 0,
    }

    public enum ImportOption
    {
        NewCollection,
        AddToCollection,
    }
    public enum ExportImagesOption
    {
        OneByGathererId,
        OneByCardName,
    }
    public enum PriceSource
    {
        MTGGoldfish
    }

    [Flags]
    public enum CardSubType
    {
        None = 0,
        Vehicle = 1,
        Host = 1 << 1,
        Aura = 1 << 2,
        Snow = 1 << 3,
        Legendary = 1 << 4,
        Curse = 1 << 5,
        Trap = 1 << 6,
        Arcane = 1 << 7,
        Tribal = 1 << 8,
        Saga = 1 << 9,
        Adventure = 1 << 10,
        Equipment = 1 << 11,
        //Must be constistante with MagicRules.GetCardSubType
    }

    [Flags]
    public enum CardType
    {
        Token = 0,
        Land = 1,
        Instant = 1 << 1,
        Sorcery = 1 << 2,
        Enchantment = 1 << 3,
        Creature = 1 << 4,
        Artifact = 1 << 5,
        Planeswalker = 1 << 6,
        Plane = 1 << 7,
        Scheme = 1 << 8,
        Conspiracy = 1 << 9,
        Phenomenon = 1 << 10,
        Contraption = 1 << 11,
        Vanguard = 1 << 12,
        //Must be constistante with MagicRules.GetCardType
    }

    [Flags]
    public enum ShardColor
    {
        Colorless = 0,
        White = 1,
        Blue = 1 << 1,
        Black = 1 << 2,
        Red = 1 << 3,
        Green = 1 << 4
    }

}
