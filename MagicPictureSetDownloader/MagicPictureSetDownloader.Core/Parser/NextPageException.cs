namespace MagicPictureSetDownloader.Core
{
    using System;

    [Serializable]
    public class NextPageException : ApplicationException
    {
        public NextPageException(int[] pages)
        {
            Pages = pages;
        }
        public int[] Pages { get; private set; }
    }
}
