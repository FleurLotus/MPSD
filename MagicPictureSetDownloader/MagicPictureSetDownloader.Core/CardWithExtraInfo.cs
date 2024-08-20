namespace MagicPictureSetDownloader.Core
{
    using System.Collections.Generic;


    internal class CardWithExtraInfo
    {
        private readonly HashSet<string> _otherIdScryFall = new HashSet<string>();

        public string IdScryFall { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
        public string Power { get; set; }
        public string Toughness { get; set; }
        public string CastingCost { get; set; }
        public string Loyalty { get; set; }
        public string Defense { get; set; }
        public string Type { get; set; }
        public string PictureUrl { get; set; }
        public string Rarity { get; set; }
        public IList<string> OtherIdScryFall
        {
            get { return new List<string>(_otherIdScryFall).AsReadOnly(); }
        }
        public void Add(string otherIdScryFall)
        {
            _otherIdScryFall.Add(otherIdScryFall);
        }
    }
}
