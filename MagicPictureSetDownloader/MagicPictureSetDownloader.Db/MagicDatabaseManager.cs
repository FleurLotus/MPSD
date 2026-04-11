namespace MagicPictureSetDownloader.Db
{
    using MagicPictureSetDownloader.Interface;

    public static class MagicDatabaseManager
    {
        private static MagicDatabase _magicDatabase;

        public static void Initialise()
        {
            _magicDatabase = new MagicDatabase();
            _magicDatabase.Initialize();
        }

        public static IMagicDatabaseReadOnly ReadOnly
        {
            get { return _magicDatabase; }
        }
        public static IMagicDatabaseReadAndWriteReference ReadAndWriteReference
        {
            get { return _magicDatabase; }
        }
        public static IMagicDatabaseReadAndWriteCollectionInBatch ReadAndWriteCollectionInBatch
        {
            get { return _magicDatabase; }
        }
        public static IMagicDatabaseReadAndWriteCardInCollectionInBatch ReadAndWriteCardInCollectionInBatch
        {
            get { return _magicDatabase; }
        }
        public static IMagicDatabaseReadAndWriteOption ReadAndWriteOption
        {
            get { return _magicDatabase; }
        }
        public static IMagicDatabaseReadAndUpdate ReadAndUpdate
        {
            get { return _magicDatabase; }
        }
    }
}