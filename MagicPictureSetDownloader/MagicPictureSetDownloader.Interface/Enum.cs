namespace MagicPictureSetDownloader.Interface
{
    using System;

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
        MPSD2,
        MPSD
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
    public enum CardIdSource
    {
        Mtgo,
        MtgoFoil,
        Multiverse,
        Tcgplayer,
        TcgplayerEtched,
        Cardmarket,
    }
    public enum PriceSource
    {
        Scryfall,
    }
    public enum PriceValueSource
    {
        Unknown,
        Cardmarket,
        TCGplayer
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
        Kindred = 1 << 8,
        Saga = 1 << 9,
        Adventure = 1 << 10,
        Equipment = 1 << 11,
        Siege = 1 << 12,
        Room = 1 << 13,
        Omen = 1 << 14,
        Food = 1 << 15,
        Clue = 1 << 16,
        Lesson = 1 << 17,
        Quest = 1 << 18,
        Class = 1 << 19,
        Case = 1 << 20,
        Locus = 1 << 21,
        Lair = 1 << 22,
        Town = 1 << 23,
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
        Stickers = 1 << 13,
        Battle = 1 << 14,
        Dungeon = 1 << 15,
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