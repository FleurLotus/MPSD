namespace ScryfallTest.Db
{
    using System.Diagnostics;

    using Common.Database;

    [DebuggerDisplay("{Name}")]
    [DbTable]
    internal class Block : IBlock
    {
        [DbColumn(Kind = ColumnKind.Identity)]
        public int Id { get; set; }
        [DbColumn]
        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
