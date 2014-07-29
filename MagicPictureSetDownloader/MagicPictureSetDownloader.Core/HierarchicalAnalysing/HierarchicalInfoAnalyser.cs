namespace MagicPictureSetDownloader.Core.HierarchicalAnalysing
{
    using System;
    public class HierarchicalInfoAnalyser: IHierarchicalInfoAnalyser
    {
        private readonly Func<ICardInfo, IComparable> _method;

        internal HierarchicalInfoAnalyser(Func<ICardInfo, IComparable> method)
        {
            _method = method;
        }

        public IComparable Analyse(ICardInfo cardVieModel)
        {
            return _method(cardVieModel);
        }
    }
}
