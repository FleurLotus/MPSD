namespace Common.SQL
{
    using System;

    internal class ColumnForForeignKey : IColumnForForeignKey, IComparable<IColumnForForeignKey>
    {
        public IColumn SourceColumn { get; set; }
        public IColumn ReferenceColumn { get; set; }
        public int Position { get; set; }

        public int CompareTo(IColumnForForeignKey other)
        {
            return Position.CompareTo(other.Position);
        }

        public override string ToString()
        {
            return $"{Position} {SourceColumn} -> {ReferenceColumn}";
        }
    }
}