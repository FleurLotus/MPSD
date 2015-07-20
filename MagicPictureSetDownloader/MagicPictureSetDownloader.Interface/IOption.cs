namespace MagicPictureSetDownloader.Interface
{
    public interface IOption
    {
        int Id { get; }
        string Key { get; }
        string Value { get; }
        TypeOfOption Type { get; }
    }
}
