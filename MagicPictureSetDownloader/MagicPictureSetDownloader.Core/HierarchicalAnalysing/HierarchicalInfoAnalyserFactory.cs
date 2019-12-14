namespace MagicPictureSetDownloader.Core.HierarchicalAnalysing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class HierarchicalInfoAnalyserFactory
    {
        private static readonly Lazy<HierarchicalInfoAnalyserFactory> _lazy = new Lazy<HierarchicalInfoAnalyserFactory>(() => new HierarchicalInfoAnalyserFactory());
        private readonly IDictionary<string, IHierarchicalInfoAnalyser> _analyser;

        private HierarchicalInfoAnalyserFactory()
        {
            _analyser = new Dictionary<string, IHierarchicalInfoAnalyser>
                            {
                                { "Color", new HierarchicalInfoAnalyser(GetColor) },
                                { "CastingCost", new HierarchicalInfoAnalyser(GetConvertedCastCost) },
                                { "Type", new HierarchicalInfoAnalyser(GetCardType) },
                                { "Edition", new HierarchicalInfoAnalyser(GetEdition) },
                                { "Rarity", new HierarchicalInfoAnalyser(GetRarity) }
                            };
        }

        public static HierarchicalInfoAnalyserFactory Instance
        {
            get { return _lazy.Value; }
        }

        public string[] Names
        {
            get
            {
                return _analyser.Keys.ToArray();
            }
        }

        public IHierarchicalInfoAnalyser Create(string name)
        {
            return _analyser[name];
        }

        private static IComparable GetEdition(ICardInfo card)
        {
            return card.Edition;
        }
        private static IComparable GetRarity(ICardInfo card)
        {
            return card.Rarity;
        }
        private static IComparable GetColor(ICardInfo card)
        {
            if (MagicRules.IsLand(card.Type))
            {
                return DisplayColor.Land;
            }

            if (MagicRules.IsSpecial(card.Type))
            {
                return DisplayColor.Special;
            }

            return MagicRules.GetDisplayColor(card.AllPartCastingCost);
        }
        private static IComparable GetConvertedCastCost(ICardInfo card)
        {
            return MagicRules.GetConvertedCastCost(card.AllPartCastingCost);
        }
        private static IComparable GetCardType(ICardInfo card)
        {
            return MagicRules.GetDisplayCardType(card.Type, card.CastingCost);
        }
    }
}
