namespace MagicPictureSetDownloader.ViewModel.Main
{
    using System;

    public class HierarchicalResultNodeViewModel : HierarchicalResultViewModel
    {
        public HierarchicalResultNodeViewModel(IComparable value, int idGatherer)
            : base(value)
        {
            IdGatherer = idGatherer;
        }
        public int IdGatherer { get; private set; }
    }
}
