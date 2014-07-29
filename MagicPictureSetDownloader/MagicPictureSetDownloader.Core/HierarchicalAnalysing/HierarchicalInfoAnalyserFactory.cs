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
                                { "Color", new HierarchicalInfoAnalyser(MagicRules.GetColor) },
                                { "CastingCost", new HierarchicalInfoAnalyser(MagicRules.GetConvertedCastCost) },
                                { "Type", new HierarchicalInfoAnalyser(MagicRules.GetCardType) },
                                { "Edition", new HierarchicalInfoAnalyser(MagicRules.GetEdition) },
                                { "Rarity", new HierarchicalInfoAnalyser(MagicRules.GetRarity) }
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

    }
}
