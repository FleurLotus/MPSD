
namespace Common.Library.Html
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class HtmlTable : IHtmlTable
    {
        private readonly IList<IHtmlCell[]> _rows = new List<IHtmlCell[]>();
        
        public HtmlTable(IEnumerable<IHtmlCell[]> rows)
        {
            if (rows == null)
                throw new ArgumentNullException("rows");

            Build(rows);
        }

        public int RowCount
        {
            get { return _rows.Count; }
        }
        public int GetColCount(int row)
        {
            if (row < 0 || row >= _rows.Count)
                return 0;

            return _rows[row].Length;
        }

        public IHtmlCell this[int row, int col]
        {
            get
            {
                if (row < 0 || row >= _rows.Count)
                    return null;

                if (col < 0 || col >= _rows[row].Length)
                    return null;

                return _rows[row][col];
            }
        }

        private void Build(IEnumerable<IHtmlCell[]> rows)
        {
            Queue<IList<IHtmlCell>> waitingRows = new Queue<IList<IHtmlCell>>();

            foreach (var cells in rows)
            {
                IList<IHtmlCell> currentRow = waitingRows.Count > 0 ? waitingRows.Dequeue() : new List<IHtmlCell>();

                foreach (IHtmlCell cell in cells)
                {
                    int index = InsertCellAtIndex(currentRow, cell);

                    if (cell.ColSpan > 1)
                    {
                        for (int c = 1; c < cell.ColSpan; c++)
                            InsertCellAtIndex( currentRow, cell, index + c);
                    }

                    if (cell.RowSpan > 1)
                    {
                        IList<IHtmlCell>[] waitingRowAsArray = waitingRows.ToArray();

                        for (int r = 1; r < cell.RowSpan; r++)
                        {
                            IList<IHtmlCell> workingRow = r <= waitingRowAsArray.GetLength(0) ? waitingRowAsArray[r - 1] : new List<IHtmlCell>();

                            for (int c = 0; c < cell.ColSpan; c++)
                                InsertCellAtIndex(workingRow, cell, index + c);

                            if (r > waitingRowAsArray.GetLength(0))
                                waitingRows.Enqueue(workingRow);
                        }
                    }
                }
                _rows.Add(currentRow.ToArray());
            }

            while(waitingRows.Count>0)
                _rows.Add(waitingRows.Dequeue().ToArray());

        }
        private int InsertCellAtIndex(IList<IHtmlCell> cells, IHtmlCell cell, int index = -1)
        {
            int wantedindex = index;

            if (wantedindex < 0)
            {
                for (int i = 0; i < cells.Count; i++)
                {
                    if (cells[i] == null)
                    {
                        wantedindex = i;
                        break;
                    }
                }
                if (wantedindex < 0)
                    wantedindex = cells.Count;
            }

            for (int i = cells.Count; i <= wantedindex; i++)
                cells.Add(null);

            cells[wantedindex] = cell;
            return wantedindex;
        }
    }
}
