namespace MagicPictureSetDownloader.Core
{
    using System.Collections.Generic;

    using MagicPictureSetDownloader.Interface;

    internal class CardWithExtraInfo
    {
        private readonly IDictionary<string, string> _cardLanguage = new Dictionary<string, string>();
        private readonly HashSet<int> _otherIdGatherer = new HashSet<int>();
        private string _name;

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                _cardLanguage[Constants.English] = _name;
            }
        }
        public string Text { get; set; }
        public string Power { get; set; }
        public string Toughness { get; set; }
        public string CastingCost { get; set; }
        public string Loyalty { get; set; }
        public string Defense { get; set; }
        public string Type { get; set; }
        public string PictureUrl { get; set; }
        public string Rarity { get; set; }
        public string PartName { get; set; }
        public string OtherPathName { get; set; }
        public IList<int> OtherIdGatherer
        {
            get { return new List<int>(_otherIdGatherer).AsReadOnly(); }
        }
        public IDictionary<string, string> Languages
        {
            get { return new Dictionary<string, string>(_cardLanguage); }
        }
        public void Add(int otherIdGatherer)
        {
            _otherIdGatherer.Add(otherIdGatherer);
        }

        public void Add(CardLanguageInfo language)
        {
            _cardLanguage[language.Language] = language.Name;
        }
    }
}
