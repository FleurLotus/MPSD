namespace Common.SQL
{
    internal class ColumnForForeignKey : IColumnForForeignKey
    {
        public IColumn SourceColumn { get; set; }
        public int SourcePosition { get; set; }
        public IColumn ReferenceColumn { get; set; }
        public int ReferencePosition { get; set; }

        public override string ToString()
        {
            return $"{SourcePosition} {SourceColumn} -> {ReferencePosition} {ReferenceColumn}";
        }
    }
}