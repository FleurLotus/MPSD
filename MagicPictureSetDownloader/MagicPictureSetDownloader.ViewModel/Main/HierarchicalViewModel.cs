namespace MagicPictureSetDownloader.ViewModel.Main
{
    using System;
    using System.Collections.Generic;
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
        public void MakeHierarchyAsync(IHierarchicalInfoAnalyser[] analysers, bool[] orders, IEnumerable<ICardInfo> cards)
        {
            ThreadPool.QueueUserWorkItem(o => MakeHierarchy(analysers, orders, cards));
        }
        private void MakeHierarchy(IHierarchicalInfoAnalyser[] analysers, bool[] orders, IEnumerable<ICardInfo> cards)
        {
            Root[0].Children.Clear();

            foreach (ICardInfo card in cards)
            {
                MakeHierarchy(analysers, orders, card);
            }
        }
        private void MakeHierarchy(IHierarchicalInfoAnalyser[] analysers, bool[] orders, ICardInfo card)
        {
            HierarchicalResultViewModel current = Root[0];

            for (int index = 0; index <= analysers.Length; index++)
            {
                IList<HierarchicalResultViewModel> children = current.Children;
                HierarchicalResultViewModel next = null;

                IComparable value = index < analysers.Length ? analysers[index].Analyse(card) : card.Name;

                bool isAscendentOrder = index >= orders.Length || orders[index];
                int i;

                for (i = 0; i < children.Count; i++)
                {
                    int result = value.CompareTo(children[i].Value);
                    if (result == 0)
                    {
                        next = children[i];
                        break;
                    }

                    if ((result < 0) == isAscendentOrder)
                    {
                        break;
                    }
                }

                if (next == null)
                {
                    next = index < analysers.Length ? new HierarchicalResultViewModel(value) : new HierarchicalResultNodeViewModel(value, card.IdGatherer);
                    current.Children.Insert(i, next);
                }

                current = next;
            }
        }
    }
}
