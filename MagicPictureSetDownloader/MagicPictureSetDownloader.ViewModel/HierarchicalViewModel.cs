namespace MagicPictureSetDownloader.ViewModel
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    using Common.ViewModel;
    using MagicPictureSetDownloader.Core.HierarchicalAnalysing;

    public class HierarchicalViewModel: NotifyPropertyChangedBase
    {
        private HierarchicalResultViewModel _selected;

        public HierarchicalViewModel(string name)
        {
            Root = (new List<HierarchicalResultViewModel>{ new HierarchicalResultViewModel(name)}).AsReadOnly();
        }

        public IList<HierarchicalResultViewModel> Root { get; private set; }
        public HierarchicalResultViewModel Selected
        {
            get { return _selected; }
            set
            {
                if (value != _selected)
                {
                    _selected = value;
                    OnNotifyPropertyChanged(() => Selected);
                }
            }
        }
        public void MakeHierarchyAsync(IEnumerable<IHierarchicalInfoAnalyser> analysers, IEnumerable<ICardInfo> cards)
        {
            ThreadPool.QueueUserWorkItem(o => MakeHierarchy(analysers, cards));
        }
        private void MakeHierarchy(IEnumerable<IHierarchicalInfoAnalyser> analysers, IEnumerable<ICardInfo> cards)
        {
            IHierarchicalInfoAnalyser[] hierarchicalInfoAnalysers = analysers.ToArray();
            foreach (ICardInfo card in cards)
            {
                MakeHierarchy(hierarchicalInfoAnalysers, card);
            }
        }
        private void MakeHierarchy(IEnumerable<IHierarchicalInfoAnalyser> analysers, ICardInfo card)
        {
            HierarchicalResultViewModel current = Root[0];

            foreach (IHierarchicalInfoAnalyser analyser in analysers)
            {
                string value = analyser.Analyse(card);

                HierarchicalResultViewModel next = current.Children.FirstOrDefault(hrvm => hrvm.Name == value);
                if (next == null)
                {
                    next = new HierarchicalResultViewModel(value);
                    current.Children.Add(next);
                }
                current = next;
            }
        }
    }
}
