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
            int comp = 0;
            if (string.IsNullOrEmpty(Column.SchemaName))
            {
                if (!string.IsNullOrEmpty(other.Column.SchemaName))
                {
                    comp = -1;
                }
            }
            else
            {
                comp = string.Compare(Column.SchemaName, other.Column.SchemaName, StringComparison.Ordinal);
            }

            if (comp == 0)
            {
                comp = string.Compare(Column.TableName, other.Column.TableName, StringComparison.Ordinal);
            }
            if (comp == 0)
            {
                comp = Position.CompareTo(other.Position);
            }
            return comp;
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