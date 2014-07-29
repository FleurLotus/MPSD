namespace MagicPictureSetDownloader.Core.HierarchicalAnalysing
{
    using System;
    public class MethodHierarchicalInfoAnalyser: IHierarchicalInfoAnalyser
    {
        private readonly Func<ICardInfo, string> _method;

        internal MethodHierarchicalInfoAnalyser(Func<ICardInfo, string> method)
        {
            _method = method;
        }

        public string Analyse(ICardInfo cardVieModel)
        {
            return _method(cardVieModel);
        }
    }
}
