namespace ScryfallTest.Db
{
    using System.Diagnostics;
    using Common.Database;

    [DebuggerDisplay("{Name}")]
    [DbTable]
    internal class Rarity
    {
        [DbColumn(Kind = ColumnKind.Identity)]
        public int Id { get; set; }
        [DbColumn]
        public string Name { get; set; }
        [DbColumn]
        public string Code { get; set; }

        public override string ToString()
        {
            return Name;
        }

        public int CompareTo(object obj)
        {
            if (obj is not Rarity r)
            {
                return -1;
            }

            return Id.CompareTo(r.Id);
        }
    }
}
