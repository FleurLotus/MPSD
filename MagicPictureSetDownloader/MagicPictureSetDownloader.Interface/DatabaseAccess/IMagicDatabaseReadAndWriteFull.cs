namespace MagicPictureSetDownloader.Interface
{
    using System.Diagnostics.CodeAnalysis;

    [SuppressMessage("ReSharper", "RedundantExtendsListEntry")]
    public interface IMagicDatabaseReadAndWriteFull : IMagicDatabaseReadAndWriteCollection,
                                                      IMagicDatabaseReadAndWriteReference,
                                                      IMagicDatabaseReadAndWriteOption,
                                                      IMagicDatabaseReadAndWriteCardInCollection,
                                                      IMagicDatabaseReadAndUpdate,
                                                      IMagicDatabaseReadOnly
    {
    }
}
