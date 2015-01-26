namespace MagicPictureSetDownloader.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Common.Libray.Enums;

    using MagicPictureSetDownloader.Core.CardInfo;
    
    public enum DisplayCardType
    {
        Instant,
        Sorcery,
        Enchantment,
        Creature,
        Planeswalker,
        Artifact,
        Land,
        Token
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
        Tribal = 1 << 7,
        Arcane = 1 << 8,
        Legendary = 1 << 9,
        //Must be constistante with GetAllType
    }

    public enum DisplayColor
    {
        Colorless,
        White,
        Blue,
        Black,
        Red,
        Green,
        Multicolor
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

    public static class MagicRules
    {
        private const string White = "W";
        private const string Blue = "U";
        private const string Black = "B";
        private const string Red = "R";
        private const string Green = "G";

        public static DisplayColor GetDisplayColor(string castingCost)
        {
            ShardColor color = GetColor(castingCost);
            if (color == ShardColor.Colorless) return DisplayColor.Colorless;
            if (color == ShardColor.White) return DisplayColor.White;
            if (color == ShardColor.Blue) return DisplayColor.Blue;
            if (color == ShardColor.Black) return DisplayColor.Black;
            if (color == ShardColor.Red) return DisplayColor.Red;
            if (color == ShardColor.Green) return DisplayColor.Green;
            return DisplayColor.Multicolor;
        }
        public static int GetConvertedCastCost(string castingCost)
        {
            int ccm = 0;
            foreach (string shard in GetShards(castingCost))
            {
                if (shard == White || shard == Blue || shard == Black || shard == Red || shard == Green)
                {
                    ccm += 1;
                }
                else
                {
                    int newccm;
                    if (int.TryParse(shard, out newccm))
                    {
                        ccm += newccm;
                    }
                    else if (shard.StartsWith("2"))
                    {
                        ccm += 2;
                    }
                    else
                    {
                        ccm ++;
                    }
                }
            }

            return ccm;
        }
        public static DisplayCardType GetDisplayCardType(string type)
        {
            CardType card = GetCardType(type);

            if (Matcher<CardType>.HasValue(card, CardType.Land)) return DisplayCardType.Land;
            if (Matcher<CardType>.HasValue(card, CardType.Artifact)) return DisplayCardType.Artifact;
            if (Matcher<CardType>.HasValue(card, CardType.Planeswalker)) return DisplayCardType.Planeswalker;
            if (Matcher<CardType>.HasValue(card, CardType.Enchantment)) return DisplayCardType.Enchantment;
            if (Matcher<CardType>.HasValue(card, CardType.Instant)) return DisplayCardType.Instant;
            if (Matcher<CardType>.HasValue(card, CardType.Sorcery)) return DisplayCardType.Sorcery;
            if (Matcher<CardType>.HasValue(card, CardType.Creature)) return DisplayCardType.Creature;

            return DisplayCardType.Token;
        }
        private static IEnumerable<string> GetShards(string castingCost)
        {
            if (string.IsNullOrWhiteSpace(castingCost)) return new string[0];

            return
                castingCost.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                           .Select(s => s.StartsWith(SymbolParser.Prefix) ? s.Substring(SymbolParser.Prefix.Length) : s);
        }
        private static ShardColor GetShardColor(string shard)
        {
            if (string.IsNullOrWhiteSpace(shard)) return ShardColor.Colorless;

            string shardup = shard.ToUpperInvariant();
            if (shardup.Contains(White)) return ShardColor.White;
            if (shardup.Contains(Blue)) return ShardColor.Blue;
            if (shardup.Contains(Black)) return ShardColor.Black;
            if (shardup.Contains(Red)) return ShardColor.Red;
            if (shardup.Contains(Green)) return ShardColor.Green;

            return ShardColor.Colorless;
        }

        //For search 
        public static CardType GetCardType(string type)
        {
            CardType cardType = CardType.Token;
            if (IsLand(type)) cardType |= CardType.Land;
            if (IsInstant(type)) cardType |= CardType.Instant;
            if (IsSorcery(type)) cardType |= CardType.Sorcery;
            if (IsEnchantment(type)) cardType |= CardType.Enchantment;
            if (IsCreature(type)) cardType |= CardType.Creature;
            if (IsArtifact(type)) cardType |= CardType.Artifact;
            if (IsPlaneswalker(type)) cardType |= CardType.Planeswalker;
            if (IsTribal(type)) cardType |= CardType.Tribal;
            if (IsArcane(type)) cardType |= CardType.Arcane;
            if (IsLegendary(type)) cardType |= CardType.Legendary;
            
            return cardType;
        }
        public static ShardColor GetColor(string castingCost)
        {
            return GetShards(castingCost).Select(GetShardColor)
                                         .Aggregate(ShardColor.Colorless, (current, newColor) => current | newColor);
        }
        public static bool IsSorcery(string type)
        {
            return type.ToLowerInvariant().Contains("sorcery");
        }
        public static bool IsInstant(string type)
        {
            return type.ToLowerInvariant().Contains("instant") || type.ToLowerInvariant().Contains("interrupt");
        }
        public static bool IsEnchantment(string type)
        {
            return type.ToLowerInvariant().Contains("enchant");
        }
        public static bool IsLand(string type)
        {
            return type.ToLowerInvariant().Contains("land");
        }
        public static bool IsPlaneswalker(string type)
        {
            return type.ToLowerInvariant().Contains("planeswalker");
        }
        public static bool IsArtifact(string type)
        {
            return type.ToLowerInvariant().Contains("artifact");
        }
        public static bool IsCreature(string type)
        {
            return type.ToLowerInvariant().Contains("summon") || type.ToLowerInvariant().Contains("eaturecray") || 
                  (type.ToLowerInvariant().Contains("creature") && !type.ToLowerInvariant().Contains("enchant creature"));
        }
        public static bool IsTribal(string type)
        {
            return type.ToLowerInvariant().Contains("tribal");
        }
        public static bool IsArcane(string type)
        {
            return type.ToLowerInvariant().Contains("arcane");
        }
        public static bool IsLegendary(string type)
        {
            return type.ToLowerInvariant().Contains("legend");
        }
    }
}
