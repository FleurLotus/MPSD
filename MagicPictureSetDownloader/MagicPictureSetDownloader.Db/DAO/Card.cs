namespace MagicPictureSetDownloader.Db.DAO
{
    using System.Collections.Generic;

    using Common.Database;
    using Common.Libray;

    using MagicPictureSetDownloader.Interface;

    [DbTable]
    internal class Card : ICard
    {
        private readonly IDictionary<int, string> _translations = new Dictionary<int, string>(); 

        [DbColumn]
        [DbKeyColumn(true)]
        public int Id { get; set; }
        [DbColumn]
        public string Name { get; set; }
        [DbColumn]
        public string Text { get; set; }
        [DbColumn]
        public string Power { get; set; }
        [DbColumn]
        public string Toughness { get; set; }
        [DbColumn]
        public string CastingCost { get; set; }
        [DbColumn]
        public int? Loyalty { get; set; }
        [DbColumn]
        public string Type { get; set; }
        [DbColumn]
        public string PartName { get; set; }
        [DbColumn]
        public string OtherPartName { get; set; }

        public bool IsMultiPart
        {
            get { return OtherPartName != null; }
        }
        public bool IsReverseSide
        {
            get { return IsMultiPart && CastingCost == null; }
        }
        public bool IsSplitted
        {
            get { return IsMultiPart && PartName != Name && OtherPartName != Name; }
        }
        public string ToString(int? languageId)
        {
            if (languageId == null)
                return IsSplitted ? Name + PartName : Name;

            return _translations.GetOrDefault(languageId.Value);
        }
        public override string ToString()
        {
            return ToString(null);
        }

        internal void AddTranslate(Translate translate)
        {
            if (translate == null || translate.IdCard != Id)
                return;
            _translations[translate.IdLanguage] = translate.Name;
        }
        public bool HasTranslation(int languageId)
        {
            return _translations.ContainsKey(languageId);
        }
    }
}
