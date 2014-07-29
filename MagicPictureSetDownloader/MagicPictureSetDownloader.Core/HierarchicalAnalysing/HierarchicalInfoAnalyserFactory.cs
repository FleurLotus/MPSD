namespace MagicPictureSetDownloader.Core.HierarchicalAnalysing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public class HierarchicalInfoAnalyserFactory
    {
        private static readonly Lazy<HierarchicalInfoAnalyserFactory> _lazy = new Lazy<HierarchicalInfoAnalyserFactory>(() => new HierarchicalInfoAnalyserFactory());
        private readonly IDictionary<string, IHierarchicalInfoAnalyser> _analyser;

        //TODO: Manage Order in Analyser result
        private HierarchicalInfoAnalyserFactory()
        {
            _analyser = new Dictionary<string, IHierarchicalInfoAnalyser>
                            {
                                { "Color", new MethodHierarchicalInfoAnalyser(MagicRules.GetColor) },
                                { "CastCost", new MethodHierarchicalInfoAnalyser(MagicRules.GetConvertedCastCost) },
                                { "Type", new MethodHierarchicalInfoAnalyser(MagicRules.GetCardType) }
                            };

            foreach (PropertyInfo propertyInfo in typeof(ICardInfo).GetProperties())
            {
                if (!_analyser.ContainsKey(propertyInfo.Name))
                {
                    _analyser.Add(propertyInfo.Name, new PropertyHierarchicalInfoAnalyser(propertyInfo));
                }
            }
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
