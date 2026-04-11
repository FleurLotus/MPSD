namespace MagicPictureSetDownloader.Db
{
    using Common.Database;

    using MagicPictureSetDownloader.Interface;

    internal partial class MagicDatabase
    {
        private sealed class Batch : IBatch
        {
            private readonly MagicDatabase _database;
            //To avoid multiple call of dispose on the same object and break of recursivity
            private readonly object _sync = new object();
            private bool _commit;
            private bool _disposed;

            public Batch(MagicDatabase database)
            {
                _database = database;
                _database.ActivateBatchMode();
            }

            public void Commit()
            {
                _commit = true;
            }

            public void Dispose()
            {
                lock (_sync)
                {
                    if (_disposed)
                    {
                        return;
                    }

                    _disposed = true;
                }
                _database.DesactivateBatchMode(_commit);
            }
        }

        private void ActivateBatchMode()
        {
            _databaseConnection.ActivateBatchMode();
        }
        private void DesactivateBatchMode(bool success)
        {
            _databaseConnection.DesactivateBatchMode(success);
        }

        public IBatch BatchMode()
        {
            return new Batch(this);
        }
        private void CheckBatchModeActivated()
        {
            if (!_databaseConnection.IsBatchModeActivated())
            {
                throw new ApplicationDbException("BatchMode is not activated");
            }
        }
    }
}