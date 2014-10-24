namespace MagicPictureSetDownloader.Db
{
    using MagicPictureSetDownloader.Interface;

    public static class MagicDatabaseManager
    {
        public static IMagicDatabaseReadOnly ReadOnly 
        {
            get { return MagicDatabase.DbInstance; }
        }
        public static IMagicDatabaseReadAndWriteReference ReadAndWriteReference
        {
            get { return MagicDatabase.DbInstance; }
        }
        public static IMagicDatabaseReadAndWriteCollection ReadAndWriteCollection
        {
            get { return MagicDatabase.DbInstance; }
        }
        public static IMagicDatabaseReadAndWriteCardInCollection ReadAndWriteCardInCollection
        {
            get { return MagicDatabase.DbInstance; }
        }
        public static IMagicDatabaseReadAndWriteOption ReadAndWriteOption
        {
            get { return MagicDatabase.DbInstance; }
        }
        public static IMagicDatabaseReadAndWriteFull ReadAndWriteFull
        {
            get { return MagicDatabase.DbInstance; }
        }

    }
}
