namespace MagicPictureSetDownloader.Core.HierarchicalAnalysing
{
    using System.Reflection;

    public class HierarchicalInfoAnalyser: IHierarchicalInfoAnalyser
    {
        private readonly PropertyInfo _propertyInfo;

        internal HierarchicalInfoAnalyser(PropertyInfo propertyInfo)
        {
            _propertyInfo = propertyInfo;
        }

        public string Analyse(ICardInfo cardVieModel)
        {
            return _propertyInfo.GetValue(cardVieModel, null).ToString();
        }
    }
}
