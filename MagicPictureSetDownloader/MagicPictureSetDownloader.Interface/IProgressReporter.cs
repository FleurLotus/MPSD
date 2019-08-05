namespace MagicPictureSetDownloader.Interface
{
    public interface IProgressReporter
    {
        void Progress();
        void Finish();
    }
}