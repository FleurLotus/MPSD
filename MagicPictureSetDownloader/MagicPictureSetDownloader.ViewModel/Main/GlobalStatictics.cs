namespace MagicPictureSetDownloader.ViewModel.Main
{
    using System.Linq;

    public class GlobalStatictics
    {
        private readonly string _name;

        private int _countTotal;
        private int _countDistinct;

        public GlobalStatictics(string name)
        {
            _name = name;
            Reset();
        }
        public void Add(CardViewModel card)
        {
            _countDistinct++;

            //Statitics are Card name based not idgatherer based, we need to filter 
            int toAdd = card.Statistics.Where(s => s.Collection == _name && s.Edition == card.Edition.Name).Aggregate(0, (p, s) => p + s.FoilNumber + s.Number);
            if (toAdd > 0)
            {
                _countTotal += toAdd;
            }
        }

        public void Reset()
        {
            _countTotal = 0;
            _countDistinct = 0;
        }
        public string GetInfo(bool onlyCount)
        {
            if (onlyCount)
                return _countDistinct + " distinct card(s)";

            return string.Format("{0} card(s) ({1} distinct)", _countTotal, _countDistinct);
        }
    }
}
