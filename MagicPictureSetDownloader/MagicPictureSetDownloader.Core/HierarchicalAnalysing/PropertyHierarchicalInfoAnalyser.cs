namespace MagicPictureSetDownloader.Core.HierarchicalAnalysing
{
    using System.Reflection;

    public class PropertyHierarchicalInfoAnalyser: IHierarchicalInfoAnalyser
    {
        private readonly PropertyInfo _propertyInfo;

        internal PropertyHierarchicalInfoAnalyser(PropertyInfo propertyInfo)
        {
            _propertyInfo = propertyInfo;
        }

        public string Analyse(ICardInfo cardVieModel)
        {
            return _propertyInfo.GetValue(cardVieModel, null).ToString();
        }
    }
}
