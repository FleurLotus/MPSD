namespace MagicPictureSetDownloader.ViewModel.Main
{
    using System;
    using System.Collections.Generic;
    using Common.ViewModel;

    using MagicPictureSetDownloader.Core.HierarchicalAnalysing;

    public class HierarchicalViewModel: NotifyPropertyChangedBase
    {
        private enum Matching
        {
            None,
            Name,
            Full,
        }

        private HierarchicalResultViewModel _selected;
        private readonly Func<string, IEnumerable<CardViewModel>> _getCardViewModels;
        //Use temporary to notify change only after build because of asynchronous call
        private readonly HierarchicalResultViewModel _buildingRoot;
        private IList<HierarchicalResultViewModel> _root;
        private readonly GlobalStatictics _globalStatictics;

        public HierarchicalViewModel(string name, Func<string, IEnumerable<CardViewModel>> getCardViewModels)
        {
            Name = name;
            _globalStatictics = new GlobalStatictics(name);
            _getCardViewModels = getCardViewModels;
            _buildingRoot = new HierarchicalResultViewModel(name);
            Root = new List<HierarchicalResultViewModel>();
        }

        public string Name { get; private set; }

        public IList<HierarchicalResultViewModel> Root
        {
            get { return _root; }
            private set
            {
                if (value != _root)
                {
                    _root = value;
                    OnNotifyPropertyChanged(() => Root);
                }
            }
        }
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
        public void MakeHierarchy(IHierarchicalInfoAnalyser[] analysers, bool[] orders)
        {
            CardViewModel saveSelected = null;
            HierarchicalResultNodeViewModel selected = Selected as HierarchicalResultNodeViewModel;
            if (selected != null)
            {
                saveSelected = selected.Card;
            }

            _buildingRoot.Children.Clear();
            _globalStatictics.Reset();

            foreach (CardViewModel card in _getCardViewModels(Name))
            {
                MakeHierarchy(analysers, orders, card);
                _globalStatictics.Add(card);
            }
            
            Root = (new List<HierarchicalResultViewModel> { _buildingRoot });

            if (saveSelected != null)
            {
                HierarchicalResultNodeViewModel bestmatch = FindBestName(_buildingRoot, saveSelected);
                if (bestmatch != null)
                {
                    Selected = bestmatch;
                }
            }
        }

        private void MakeHierarchy(IHierarchicalInfoAnalyser[] analysers, bool[] orders, CardViewModel card)
        {
            HierarchicalResultViewModel current = _buildingRoot;

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
                    next = index < analysers.Length ? new HierarchicalResultViewModel(value) : new HierarchicalResultNodeViewModel(value);
                    current.Children.Insert(i, next);
                }

                if (index == analysers.Length)
                {
                    // ReSharper disable PossibleNullReferenceException
                    (next as HierarchicalResultNodeViewModel).AddCard(card);
                    // ReSharper restore PossibleNullReferenceException
                }

                current = next;
            }
        }
        private Matching FindBestMatch(HierarchicalResultNodeViewModel hrnvm, CardViewModel saveSelected)
        {
            if (hrnvm == null || hrnvm.Card == null)
            {
                return Matching.None;
            }

            CardViewModel card = hrnvm.Card;

            if (card.Name != saveSelected.Name)
            {
                return Matching.None;
            }

            return card.IdGatherer == saveSelected.IdGatherer ? Matching.Full : Matching.Name;
        }
        private HierarchicalResultNodeViewModel FindBestName(HierarchicalResultViewModel toInspect, CardViewModel saveSelected)
        {
            if (toInspect == null || saveSelected == null)
            {
                return null;
            }

            HierarchicalResultNodeViewModel res = null;
            HierarchicalResultNodeViewModel nodevm = toInspect as HierarchicalResultNodeViewModel;
            Matching resMatch = FindBestMatch(nodevm, saveSelected);
            if (resMatch == Matching.Full)
            {
                return nodevm;
            }

            if (resMatch == Matching.Name)
            {
                res = nodevm;
            }

            foreach (HierarchicalResultViewModel child in toInspect.Children)
            {
                nodevm = FindBestName(child, saveSelected);

                resMatch = FindBestMatch(nodevm, saveSelected);
                if (resMatch == Matching.Full)
                {
                    return nodevm;
                }

                if (resMatch == Matching.Name && res == null)
                {
                    res = nodevm;
                }
            }

            return res;
        }

        internal string GetInfo(bool onlyCount)
        {
            return _globalStatictics.GetInfo(onlyCount);
        }
    }
}
