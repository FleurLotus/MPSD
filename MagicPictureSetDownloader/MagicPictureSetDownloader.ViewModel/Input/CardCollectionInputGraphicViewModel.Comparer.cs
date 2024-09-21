namespace MagicPictureSetDownloader.ViewModel.Input
{
    using System.Collections.Generic;

    using MagicPictureSetDownloader.Interface;
    using MagicPictureSetDownloader.Core;

    public enum DisplayOrder
    {
        Name,
        LanguageName,
        Custom
    }
    public partial class CardCollectionInputGraphicViewModel
    {
        private static readonly IDictionary<DisplayOrder, IComparer<CardCollectionInputGraphicViewModel>> Orderer = new Dictionary<DisplayOrder, IComparer<CardCollectionInputGraphicViewModel>>
        {
            {DisplayOrder.Name, new NameComparer()},
            {DisplayOrder.LanguageName, new LanguageNameComparer()},
            {DisplayOrder.Custom, new CustomComparer()},
        };

        internal class NameComparer : IComparer<CardCollectionInputGraphicViewModel>
        {
            public int Compare(CardCollectionInputGraphicViewModel x, CardCollectionInputGraphicViewModel y)
            {
                return x.Name.CompareTo(y.Name);
            }
        }
        internal class LanguageNameComparer : IComparer<CardCollectionInputGraphicViewModel>
        {
            public int Compare(CardCollectionInputGraphicViewModel x, CardCollectionInputGraphicViewModel y)
            {
                return x.NameInLanguage.CompareTo(y.NameInLanguage);
            }
        }
        internal class CustomComparer : IComparer<CardCollectionInputGraphicViewModel>
        {
            private readonly IComparer<CardCollectionInputGraphicViewModel> _languageNameComparer = new LanguageNameComparer();
            private readonly IComparer<ICardAllDbInfo> _cardComparer = new CustomCardComparer();
            public int Compare(CardCollectionInputGraphicViewModel x, CardCollectionInputGraphicViewModel y)
            {
                int comp = _cardComparer.Compare(x.Card.CardAllDbInfo, y.Card.CardAllDbInfo);
                if (comp != 0)
                {
                    return comp;
                }

                return _languageNameComparer.Compare(x, y);
            }
        }

        public static IComparer<CardCollectionInputGraphicViewModel> GetComparer(DisplayOrder displayOrder)
        {
            return Orderer[displayOrder];
        }
    }
}
