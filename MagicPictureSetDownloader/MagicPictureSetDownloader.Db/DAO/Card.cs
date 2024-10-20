﻿namespace MagicPictureSetDownloader.Db.DAO
{
    using System.Linq;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Common.Database;
    using Common.Library.Extension;

    using MagicPictureSetDownloader.Interface;

    [DbTable]
    internal class Card : ICard
    {
        private readonly IDictionary<int, string> _translations = new Dictionary<int, string>();

        private readonly List<ICardFace> _faces = new List<ICardFace>();

        [DbColumn(Kind = ColumnKind.Identity)]
        public int Id { get; set; }
        [DbColumn]
        public string Name { get; set; }
        [DbColumn]
        public string Layout { get; set; }

        public ICardFace MainCardFace
        {
            get { return _faces.Count > 0 ? _faces[0] : null; }
        }

        public ICardFace OtherCardFace
        {
            get { return _faces.Count == 2 ? _faces[1] : null; }
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
        public bool HasCardFace(string name)
        {
            return _faces.Any(f=> f.Name == name);
        }

        internal void AddCardFace(CardFace cardFace)
        {
            if (cardFace == null)
            {
                return;
            }
            _faces.Add(cardFace);

            if (_faces.Count > 2)
            {
                //Not Normal
                Debugger.Break();
            }
        }
    }
}
