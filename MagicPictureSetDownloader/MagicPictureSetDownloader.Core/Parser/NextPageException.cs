namespace MagicPictureSetDownloader.Core
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class NextPageException : ApplicationException
    {
        public NextPageException(int[] pages)
        {
            Pages = pages;
        }
        public int[] Pages { get; }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Pages", Pages, typeof(int[]));
            base.GetObjectData(info, context);
        }
    }
}
