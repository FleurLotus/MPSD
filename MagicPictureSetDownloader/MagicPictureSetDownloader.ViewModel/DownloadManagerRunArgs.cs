
namespace MagicPictureSetDownloader.ViewModel
{
    public class DownloadManagerRunArgs
    {
        public string BaseSetUrl { get; private set; }
        public string OutputPath { get; private set; }

        public DownloadManagerRunArgs(string baseSetUrl, string outputPath)
        {
            BaseSetUrl = baseSetUrl;
            OutputPath = outputPath;
        }
    }
}
