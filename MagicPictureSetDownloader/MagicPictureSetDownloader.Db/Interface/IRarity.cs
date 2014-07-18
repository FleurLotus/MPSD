namespace MagicPictureSetDownloader.Db
{
    public interface IRarity
    {
        int Id { get;  }
        string Name { get; }
        string Code { get; }
    }
}