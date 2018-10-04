namespace MagicPictureSetDownloader.ViewModel.Input
{
    using System.Linq;

    using Common.ViewModel.Dialog;

    using MagicPictureSetDownloader.Db;
    using MagicPictureSetDownloader.Interface;
    
    public class UpdateViewModelCommun : DialogViewModelBase
    {
        protected readonly IMagicDatabaseReadOnly MagicDatabase;

        protected UpdateViewModelCommun(string collectionName)
        {
            MagicDatabase = MagicDatabaseManager.ReadOnly;

            SourceCollection = MagicDatabase.GetAllCollections().First(cc => cc.Name == collectionName);

            Display.OkCommandLabel = "Update";
            Display.CancelCommandLabel = "Close";
        }
        public ICardCollection SourceCollection { get; }
    }
}
