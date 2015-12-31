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

             public Batch(MagicDatabase database)
             {
                 _database = database;
                 _database.IncrementBatchDepth();
             }

             public void Dispose()
             {
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
