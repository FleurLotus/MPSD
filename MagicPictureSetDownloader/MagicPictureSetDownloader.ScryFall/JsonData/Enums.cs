namespace MagicPictureSetDownloader.ScryFall.JsonData
{
    using System.Runtime.Serialization;
    using System.Text.Json.Serialization;
    
    [JsonConverter(typeof(JsonStringEnumMemberConverter))]
    public enum Color
    {
        [EnumMember(Value = "W")]
        White,
        [EnumMember(Value = "U")]
        Blue,
        [EnumMember(Value = "B")]
        Black,
        [EnumMember(Value = "R")]
        Red,
        [EnumMember(Value = "G")]
        Green,
    }    

    [JsonConverter(typeof(JsonStringEnumMemberConverter))]
    public enum ImageStatus
    {
        [EnumMember(Value = "missing")]
        Missing,
        [EnumMember(Value = "placeholder")]
        Placeholder,
        [EnumMember(Value = "lowres")]
        LowRes,
        [EnumMember(Value = "highres_scan")]
        HighRes,
    }    

    [JsonConverter(typeof(JsonStringEnumMemberConverter))]
    public enum Game
    {
        [EnumMember(Value = "paper")]
        Paper,
        [EnumMember(Value = "arena")]
        Arena,
        [EnumMember(Value = "mtgo")]
        Mtgo,
        [EnumMember(Value = "sega")]
        Sega,
        [EnumMember(Value = "astral")]
        Astral,
    }

    [JsonConverter(typeof(JsonStringEnumMemberConverter))]
    public enum Frame
    {
        [EnumMember(Value = "1993")]
        Year1993,
        [EnumMember(Value = "1997")]
        Year1997,
        [EnumMember(Value = "2003")]
        Year2003,
        [EnumMember(Value = "2015")]
        Year2015,
        [EnumMember(Value = "future")]
        Future,
    }
    
    [JsonConverter(typeof(JsonStringEnumMemberConverter))]
    public enum FrameEffect
    {
        [EnumMember(Value = "legendary")]
        Legendary,
        [EnumMember(Value = "miracle")]
        Miracle,
        [EnumMember(Value = "nyxtouched")]
        NyxTouched,
        [EnumMember(Value = "draft")]
        Draft,
        [EnumMember(Value = "devoid")]
        Devoid,
        [EnumMember(Value = "tombstone")]
        TombStone,
        [EnumMember(Value = "colorshifted")]
        ColorShifted,
        [EnumMember(Value = "inverted")]
        Inverted,
        [EnumMember(Value = "sunmoondfc")]
        SunMoonDfc,
        [EnumMember(Value = "compasslanddfc")]
        CompassLandDfc,
        [EnumMember(Value = "originpwdfc")]
        OriginPlaneswalkerDfc,
        [EnumMember(Value = "mooneldrazidfc")]
        MoonEldraziDfc,
        [EnumMember(Value = "waxingandwaningmoondfc")]
        WaxingAndWaningMoonDfc,
        [EnumMember(Value = "showcase")]
        ShowCase,
        [EnumMember(Value = "extendedart")]
        ExtendedArt,
        [EnumMember(Value = "companion")]
        Companion,
        [EnumMember(Value = "etched")]
        Etched,
        [EnumMember(Value = "snow")]
        Snow,
        [EnumMember(Value = "lesson")]
        Lesson,
        [EnumMember(Value = "shatteredglass")]
        ShatteredGlass,
        [EnumMember(Value = "convertdfc")]
        ConvertDfc,
        [EnumMember(Value = "fandfc")]
        FanDfc,
        [EnumMember(Value = "upsidedowndfc")]
        UpsideDownDfc,
        [EnumMember(Value = "fullart")]
        FullArt,
        [EnumMember(Value = "thick")]
        Thick,
        [EnumMember(Value = "textless")]
        Textless,
        [EnumMember(Value = "spree")]
        Spree,
    }
    [JsonConverter(typeof(JsonStringEnumMemberConverter))]
    public enum Finish
    {
        [EnumMember(Value = "foil")]
        Foil,
        [EnumMember(Value = "nonfoil")]
        NonFoil,
        [EnumMember(Value = "etched")]
        Etched,
    }
    
    [JsonConverter(typeof(JsonStringEnumMemberConverter))]
    public enum BorderColor
    {
        [EnumMember(Value = "black")]
        Black,
        [EnumMember(Value = "white")]
        White,
        [EnumMember(Value = "borderless")]
        Borderless,
        [EnumMember(Value = "silver")]
        Silver,
        [EnumMember(Value = "gold")]
        Gold,
    }

    [JsonConverter(typeof(JsonStringEnumMemberConverter))]
    public enum Component
    {
        [EnumMember(Value = "token")]
        Token,
        [EnumMember(Value = "meld_part")]
        MeldPart,
        [EnumMember(Value = "meld_result")]
        MeldResult,
        [EnumMember(Value = "combo_piece")]
        ComboPiece,
    }

    [JsonConverter(typeof(JsonStringEnumMemberConverter))]
    public enum Rarity
    {
        [EnumMember(Value = "common")]
        Common,
        [EnumMember(Value = "uncommon")]
        Uncommon,
        [EnumMember(Value = "rare")]
        Rare,
        [EnumMember(Value = "special")]
        Special,
        [EnumMember(Value = "mythic")]
        Mythic,
        [EnumMember(Value = "bonus")]
        Bonus,
    }

    [JsonConverter(typeof(JsonStringEnumMemberConverter))]
    public enum Legality
    {
        [EnumMember(Value = "legal")]
        Legal,
        [EnumMember(Value = "not_legal")]
        NotLegal,
        [EnumMember(Value = "restricted")]
        Restricted,
        [EnumMember(Value = "banned")]
        Banned,
    }

    [JsonConverter(typeof(JsonStringEnumMemberConverter))]
    public enum SetType
    {
        [EnumMember(Value = "core")]
        Core,
        [EnumMember(Value = "expansion")]
        Expansion,
        [EnumMember(Value = "masters")]
        Masters,
        [EnumMember(Value = "alchemy")]
        Alchemy,
        [EnumMember(Value = "masterpiece")]
        Masterpiece,
        [EnumMember(Value = "arsenal")]
        Arsenal,
        [EnumMember(Value = "from_the_vault")]
        FromTheVault,
        [EnumMember(Value = "spellbook")]
        Spellbook,
        [EnumMember(Value = "premium_deck")]
        PremiumDeck,
        [EnumMember(Value = "duel_deck")]
        DuelDeck,
        [EnumMember(Value = "draft_innovation")]
        DraftInnovation,
        [EnumMember(Value = "treasure_chest")]
        TreasureChest,
        [EnumMember(Value = "commander")]
        Commander,
        [EnumMember(Value = "planechase")]
        Planechase,
        [EnumMember(Value = "archenemy")]
        Archenemy,
        [EnumMember(Value = "vanguard")]
        Vanguard,
        [EnumMember(Value = "funny")]
        Funny,
        [EnumMember(Value = "starter")]
        Starter,
        [EnumMember(Value = "box")]
        Box,
        [EnumMember(Value = "promo")]
        Promo,
        [EnumMember(Value = "token")]
        Token,
        [EnumMember(Value = "memorabilia")]
        Memorabilia,
        [EnumMember(Value = "minigame")]
        Minigame,
    }

    [JsonConverter(typeof(JsonStringEnumMemberConverter))]
    public enum Layout
    {
        [EnumMember(Value = "normal")]
        Normal,
        [EnumMember(Value = "split")]
        Split,
        [EnumMember(Value = "flip")]
        Flip,
        [EnumMember(Value = "transform")]
        Transform,
        [EnumMember(Value = "modal_dfc")]
        ModalDfc,
        [EnumMember(Value = "meld")]
        Meld,
        [EnumMember(Value = "leveler")]
        Leveler,
        [EnumMember(Value = "class")]
        Class,
        [EnumMember(Value = "saga")]
        Saga,
        [EnumMember(Value = "adventure")]
        Adventure,
        [EnumMember(Value = "battle")]
        Battle,
        [EnumMember(Value = "planar")]
        Planar,
        [EnumMember(Value = "scheme")]
        Scheme,
        [EnumMember(Value = "vanguard")]
        Vanguard,
        [EnumMember(Value = "token")]
        Token,
        [EnumMember(Value = "double_faced_token")]
        DoubleFacedToken,
        [EnumMember(Value = "emblem")]
        Emblem,
        [EnumMember(Value = "augment")]
        Augment,
        [EnumMember(Value = "host")]
        Host,
        [EnumMember(Value = "art_series")]
        ArtSeries,
        [EnumMember(Value = "reversible_card")]
        ReversibleCard,
        [EnumMember(Value = "prototype")]
        Prototype,
        [EnumMember(Value = "mutate")]
        Mutate,
        [EnumMember(Value = "case")]
        Case,
    }
}