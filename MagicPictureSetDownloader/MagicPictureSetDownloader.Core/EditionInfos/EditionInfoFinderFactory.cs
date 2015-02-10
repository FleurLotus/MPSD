namespace MagicPictureSetDownloader.Core.EditionInfos
{
    using System;

    public enum IconPageType
    {
        CardKingdom,
        Wikia,
    }

    internal class EditionInfoFinderFactory
    {
        private static readonly Lazy<EditionInfoFinderFactory> _lazy = new Lazy<EditionInfoFinderFactory>(() => new EditionInfoFinderFactory());

        private EditionInfoFinderFactory()
        {
        }

        public static EditionInfoFinderFactory Instance
        {
            get { return _lazy.Value; }
        }

        public IEditionFinder CreateFinder(IconPageType pageType, Func<string,string> getHtml)
        {
            switch (pageType)
            {
                case IconPageType.CardKingdom:
                    return new EditionInfoCardKingdomFinder(getHtml);
                case IconPageType.Wikia:
                    return new EditionInfoWikiaFinder(getHtml);
                default:
                    return null;
            }
        }
    }
}
