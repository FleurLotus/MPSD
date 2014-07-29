namespace MagicPictureSetDownloader.Core.HierarchicalAnalysing
{
    using System;

    public interface IHierarchicalInfoAnalyser
    {
        IComparable Analyse(ICardInfo cardVieModel);
    }
}
