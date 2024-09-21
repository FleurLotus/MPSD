namespace MagicPictureSetDownloader.Db
{
    using MagicPictureSetDownloader.Interface;

    public static class MagicDatabaseManager
    {
        private static MagicDatabase _magicDatabase;

        public static void Initialise()
        {
            _magicDatabase = new MagicDatabase();
        }

        public static IMagicDatabaseReadOnly ReadOnly 
        {
            get { return _magicDatabase; }
        }
        public static IMagicDatabaseReadAndWriteReference ReadAndWriteReference
        {
            get { return _magicDatabase; }
        }
        public static IMagicDatabaseReadAndWriteCollection ReadAndWriteCollection
        {
            get { return _magicDatabase; }
        }
        public static IMagicDatabaseReadAndWriteCardInCollection ReadAndWriteCardInCollection
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
