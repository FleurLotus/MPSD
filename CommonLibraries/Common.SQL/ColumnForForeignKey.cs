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
            return string.Format("{0} {1} -> {2} {3}", SourcePosition, SourceColumn, ReferencePosition, ReferenceColumn);
        }
    }
}