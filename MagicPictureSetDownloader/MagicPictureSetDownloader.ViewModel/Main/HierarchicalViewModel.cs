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
        private readonly Func<string, IEnumerable<CardViewModel>> _getCardViewModels;

        public HierarchicalViewModel(string name, Func<string, IEnumerable<CardViewModel>> getCardViewModels)
        {
            Name = name;
            _getCardViewModels = getCardViewModels;
            Root = (new List<HierarchicalResultViewModel> { new HierarchicalResultViewModel(name) }).AsReadOnly();
        }

        public string Name { get; private set; }
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
        public void MakeHierarchyAsync(IHierarchicalInfoAnalyser[] analysers, bool[] orders)
        {
            ThreadPool.QueueUserWorkItem(o => MakeHierarchy(analysers, orders));
        }
        private void MakeHierarchy(IHierarchicalInfoAnalyser[] analysers, bool[] orders)
        {
            Root = (new List<HierarchicalResultViewModel> { new HierarchicalResultViewModel(Name) }).AsReadOnly();

            foreach (CardViewModel card in _getCardViewModels(Name))
            {
                MakeHierarchy(analysers, orders, card);
            }
            OnNotifyPropertyChanged(() => Root);
        }
        private void MakeHierarchy(IHierarchicalInfoAnalyser[] analysers, bool[] orders, CardViewModel card)
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
                    next = index < analysers.Length ? new HierarchicalResultViewModel(value) : new HierarchicalResultNodeViewModel(value, card);
                    current.Children.Insert(i, next);
                }
                else if (index == analysers.Length)
                {
                    //If edition is not selected in filter, multiple version of the same card could appear, keeps the descritption and so the picture of the most recent one.
                    HierarchicalResultNodeViewModel node = next as HierarchicalResultNodeViewModel;
                    if (node.Card.Edition.ReleaseDate < card.Edition.ReleaseDate)
                    {
                        next = new HierarchicalResultNodeViewModel(value, card);
                        current.Children.Remove(node);
                        current.Children.Insert(i, next);
                    }
                }

                current = next;
            }
        }
    }
}
