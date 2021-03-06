namespace MagicPictureSetDownloader.Db.DAO
{
    using System;
    using System.Linq;

    using Common.Database;
    using MagicPictureSetDownloader.Interface;

    [DbTable]
    internal class Edition : IEdition
    {
        private static readonly string[] SpecialEditionPrefix =
        {
            "MTG.WTF-",
        };

        [DbColumn(Kind = ColumnKind.Identity)]
        public int Id { get; set; }
        [DbColumn]
        public string Name { get; set; }
        [DbColumn]
        public string Code { get; set; }
        [DbColumn]
        public string AlternativeCode { get; set; }
        [DbColumn]
        public int? IdBlock { get; set; }
        public IBlock Block { get; set; }
        public string BlockName
        {
            get { return Block == null ? null : Block.Name; }
        }
        [DbColumn]
        public int? BlockPosition { get; set; }
        [DbColumn]
        public string GathererName { get; set; }
        [DbColumn]
        public DateTime? ReleaseDate { get; set; }
        [DbColumn]
        public int? CardNumber { get; set; }
        [DbColumn]
        public bool Completed { get; set; }
        [DbColumn]
        public bool HasFoil { get; set; }
        public bool IsCode(string code)
        {
            string tmp = Code + ";" + AlternativeCode;
            return tmp.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Any(c => string.Equals(c, code, StringComparison.InvariantCultureIgnoreCase));
        }

        string IEdition.AlternativeCode(ExportFormat format)
        {
            string code = Code;
            if (AlternativeCode == null || format == ExportFormat.MPSD)
            {
                return code;
            }

            string[] codes = AlternativeCode.Split(';');
            int pos = (int)format;
            if (pos < 0 || pos >= codes.Length)
            {
                return code;
            }

            if (string.IsNullOrWhiteSpace(codes[pos]))
            {
                return code;
            }

            return codes[pos].Trim();
        }

        public override string ToString()
        {
            return Name;
        }

        public int CompareTo(object obj)
        {
            Edition e = obj as Edition;

            if (e == null)
            {
                return -1;
            }

            return Name.CompareTo(e.Name);
        }
        public bool IsNoneGatherer()
        {
            string name = GathererName.ToUpper();
            return SpecialEditionPrefix.Any(p => name.StartsWith(p));
        }
    }
}
