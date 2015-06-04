
namespace MagicPictureSetDownloader.ViewModel.Management
{
    using MagicPictureSetDownloader.Interface;

    public class BlockDatabaseInfoModificationViewModel : DatabaseInfoModificationViewModelBase<IBlock>
    {
        public BlockDatabaseInfoModificationViewModel()
        {
            All.AddRange(MagicDatabase.GetAllBlocks());
            Title = "Manage Block";
        }
        protected override void DisplayCurrent()
        {
            Name = Selected == null ? null : Selected.Name;
        }
        protected override bool ApplyEditionToDatabase()
        {
            if (Selected == null)
            {
                MagicDatabase.InsertNewBlock(Name);
            }
            else
            {
                MagicDatabase.UpdateBlock(Selected, Name);
            }
            All.Clear();
            All.AddRange(MagicDatabase.GetAllBlocks());
            return true;
        }
    }
}
