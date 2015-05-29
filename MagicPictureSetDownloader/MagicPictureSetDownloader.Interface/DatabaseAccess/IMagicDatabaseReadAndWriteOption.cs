namespace MagicPictureSetDownloader.Interface
{
    public interface IMagicDatabaseReadAndWriteOption : IMagicDatabaseReadOnly
    {
        void InsertNewOption(TypeOfOption type, string key, string value);
        void DeleteOption(TypeOfOption type, string key);
    }
}
