namespace Common.SQL
{
    using System;

    internal class ColumnForIndex : IColumnForIndex, IComparable<IColumnForIndex>
    {
        public IColumn Column { get; set; }
        public bool? IsAsc { get; set; }
        public int Position { get; set; }

        public int CompareTo(IColumnForIndex other)
        {
            return Position.CompareTo(other.Position);
        }
        public override string ToString()
        {
            if (IsAsc.HasValue)
            {
                return $"{Position} {Column} {(IsAsc.Value ? "Asc" : "Desc")}";
            }
            return $"{Position} {Column}";
        }
    }
}