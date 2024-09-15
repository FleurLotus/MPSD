namespace MagicPictureSetDownloader.Db.DAO
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    using Common.Database;

    using MagicPictureSetDownloader.Interface;

    [DbTable]
    internal class CardEdition : ICardEdition
    {
        private readonly Dictionary<CardIdSource, HashSet<string>> _externalIds = new Dictionary<CardIdSource, HashSet<string>>();

        [DbColumn]
        public int IdEdition { get; set; }
        [DbColumn]
        public int IdCard { get; set; }
        [DbColumn]
        public int IdRarity { get; set; }
        [DbColumn]
        public string IdScryFall { get; set; }
        [DbColumn]
        public string Url { get; set; }
        [DbColumn]
        public string Url2 { get; set; }

        public IReadOnlyDictionary<CardIdSource, IReadOnlyList<string>> ExternalId
        {
            get
            {
                return new ReadOnlyDictionary<CardIdSource, IReadOnlyList<string>>(_externalIds.ToDictionary(kv => kv.Key, kv => (IReadOnlyList<string>)kv.Value.ToList().AsReadOnly()));
            }
        }

        internal void AddExternalId(ExternalIds externalId)
        {
            if (externalId == null || externalId.IdScryFall != IdScryFall || !Enum.TryParse(externalId.CardIdSource, out CardIdSource cardIdSource))
            {
                return;
            }

            if (!_externalIds.TryGetValue(cardIdSource, out HashSet<string> sourceIds))
            {
                sourceIds = new HashSet<string>();
                _externalIds.Add(cardIdSource, sourceIds);
            }

            sourceIds.Add(externalId.ExternalId);
        }
    }
}
