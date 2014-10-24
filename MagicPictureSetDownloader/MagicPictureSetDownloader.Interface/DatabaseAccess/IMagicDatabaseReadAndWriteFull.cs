namespace MagicPictureSetDownloader.Interface
{
    public interface IMagicDatabaseReadAndWriteFull : IMagicDatabaseReadAndWriteCollection,
                                                      IMagicDatabaseReadAndWriteReference,
                                                      IMagicDatabaseReadAndWriteOption,
                                                      IMagicDatabaseReadAndWriteCardInCollection,
                                                      IMagicDatabaseReadOnly
    {
    }
}
