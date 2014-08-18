namespace MagicPictureSetDownloader.Core.HierarchicalAnalysing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using MagicPictureSetDownloader.Core.CardInfo;

    internal static class MagicRules
    {
        private enum Color
        {
            Colorless,
            White,
            Blue,
            Black,
            Red,
            Green,
            Multicolor
        }
        
        private const string White = "W";
        private const string Blue = "U";
        private const string Black = "B";
        private const string Red = "R";
        private const string Green = "G";

        public static IComparable GetName(ICardInfo card)
        {
            return card.Name;
        }
        public static IComparable GetBlockName(ICardInfo card)
        {
            return card.BlockName;
        }
        public static IComparable GetEdition(ICardInfo card)
        {
            return card.Edition;
        }
        public static IComparable GetRarity(ICardInfo card)
        {
            return card.Rarity;
        }

        public static IComparable GetColor(ICardInfo card)
        {
            Color color = Color.Colorless;
            foreach (string shard in GetShards(card.CastingCost))
            {
                Color newColor = GetColor(shard);
                if (newColor != Color.Colorless)
                {
                    if (color != Color.Colorless && color != newColor)
                        return Color.Multicolor;

                    color = newColor;
                }
            }
            return color;
        }
        public static IComparable GetConvertedCastCost(ICardInfo card)
        {
            int ccm = 0;
            foreach (string shard in GetShards(card.CastingCost))
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
        public static IComparable GetCardType(ICardInfo card)
        {
            if (card.Type.Contains("Land"))
                return "Land";

            if (card.Type.Contains("Artifact"))
                return "Artifact";

            if (card.Type.Contains("Planeswalker"))
                return "Planeswalker";
            
            if (card.Type.Contains("Enchantment"))
                return "Enchantment";

            if (card.Type.Contains("Creature"))
                return "Creature";

            return card.Type;
        }
        private static IEnumerable<string> GetShards(string castingCost)
        {
            if (string.IsNullOrWhiteSpace(castingCost)) return new string[0];

            return
                castingCost.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                           .Select(s => s.StartsWith(SymbolParser.Prefix) ? s.Substring(SymbolParser.Prefix.Length):s);
        }
        private static Color GetColor(string shard)
        {
            if (string.IsNullOrWhiteSpace(shard)) return Color.Colorless;

            string shardup = shard.ToUpperInvariant();
            if (shardup.Contains(White)) return Color.White;
            if (shardup.Contains(Blue)) return Color.Blue;
            if (shardup.Contains(Black)) return Color.Black;
            if (shardup.Contains(Red)) return Color.Red;
            if (shardup.Contains(Green)) return Color.Green;
            
            return Color.Colorless;
        }
    }
}
