namespace MagicPictureSetDownloader.Db.DAO
{
    using System;
    using System.Linq;
    using System.Collections.Generic;

    using Common.Database;
    using Common.Library.Extension;

    using MagicPictureSetDownloader.Interface;

    [DbTable]
    internal class Card : ICard
    {
        private readonly IDictionary<int, string> _translations = new Dictionary<int, string>();
        private readonly List<int> _faces = new List<int>();

        [DbColumn(Kind = ColumnKind.Identity)]
        public int Id { get; set; }
        [DbColumn]
        public string Name { get; set; }

        public IReadOnlyList<int> CardFaceIds
        {
            get
            {
                return _faces.AsReadOnly();
            }
        }

        public string ToString(int? languageId)
        {
            if (languageId == null)
            {
                return Name;
            }

            return _translations.GetOrDefault(languageId.Value);
        }
        public override string ToString()
        {
            return ToString(null);
        }

        internal void AddTranslate(Translate translate)
        {
            if (translate == null || translate.IdCard != Id)
            {
                return;
            }

            _translations[translate.IdLanguage] = translate.Name;
        }
        public bool HasTranslation(int languageId)
        {
            return _translations.ContainsKey(languageId);
        }
        internal void AddCardFace(CardCardFace cardCardFace)
        {
            if (cardCardFace == null || cardCardFace.IdCard != Id)
            {
                return;
            }
            _faces.Add(cardCardFace.IdCardFace);
        }
    }
}
