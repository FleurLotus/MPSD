namespace MagicPictureSetDownloader.Core.HierarchicalAnalysing
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    internal static class MagicRules
    {
        private const string White = "W";
        private const string Blue = "U";
        private const string Black = "B";
        private const string Red = "R";
        private const string Green = "G";

        public static string GetColor(ICardInfo card)
        {
            string color = null;
            foreach (string shard in GetShards(card.CastingCost))
            {
                string newColor = GetColor(shard);
                if (!string.IsNullOrWhiteSpace(newColor))
                {
                    if (color != null && color != newColor) 
                        return "Multicolor";

                    color = newColor;
                }
            }
            return color ?? "Incolor";
        }
        public static string GetConvertedCastCost(ICardInfo card)
        {
            int ccm = 0;
            bool hasvalue = false;
            foreach (string shard in GetShards(card.CastingCost))
            {
                if (shard == White || shard == Blue || shard == Black || shard == Red || shard == Green)
                {
                    hasvalue = true;
                    ccm += 1;
                }
                else
                {
                    int newccm;
                    if (int.TryParse(shard, out newccm))
                    {
                        ccm += newccm;
                        hasvalue = true;
                    }
                    else if (shard.StartsWith("2"))
                    {
                        ccm += 2;
                        hasvalue = true;
                    }
                    else
                    {
                        ccm ++;
                        hasvalue = true;
                    }
                }
            }

            return hasvalue ? ccm.ToString(CultureInfo.InvariantCulture) : string.Empty;
        }
        public static string GetCardType(ICardInfo card)
        {
            if (card.Type.Contains("Creature"))
                return "Creature";

            return card.Type;
        }
        private static IEnumerable<string> GetShards(string castingCost)
        {
            if (string.IsNullOrWhiteSpace(castingCost)) return new string[0];

            return castingCost.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        }
        private static string GetColor(string shard)
        {
            if (string.IsNullOrWhiteSpace(shard)) return null;

            string shardup = shard.ToUpperInvariant();
            if (shardup.Contains(White)) return "White";
            if (shardup.Contains(Blue)) return "Blue";
            if (shardup.Contains(Black)) return "Black";
            if (shardup.Contains(Red)) return "Red";
            if (shardup.Contains(Green)) return "Green";
            
            return null;
        }
    }
}
