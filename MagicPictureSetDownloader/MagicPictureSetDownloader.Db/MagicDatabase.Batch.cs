 namespace MagicPictureSetDownloader.Db
 {
     using System;

     using Common.Library.Threading;

     internal partial class MagicDatabase
     {
         private int _depth;

         private sealed class Batch : IDisposable
         {
             private readonly MagicDatabase _database;
             //To avoid multiple call of dispose on the same object and break of recursivity
             private readonly object _sync = new object();
             private bool _disposed;

             public Batch(MagicDatabase database)
             {
                 _database = database;
                 _database.IncrementBatchDepth();
             }

             public void Dispose()
             {
                 lock (_sync)
                 {
                     if (_disposed)
                         return;

                     _disposed = true;
                 }
                 _database.DecrementBatchDepth();
             }
         }

         private void IncrementBatchDepth()
         {
             using (new WriterLock(_lock))
             {
                 if (_depth == 0)
                 {
                     _databaseConnection.ActivateBatchMode();
                 }
                 _depth++;
             }
         }
         private void DecrementBatchDepth()
         {
             using (new WriterLock(_lock))
             {
                 _depth--;
                 if (_depth == 0)
                 {
                     _databaseConnection.DesactivateBatchMode();
                 }
             }
         }


         public IDisposable BatchMode()
         {
             return new Batch(this);
         }
     }
 }
