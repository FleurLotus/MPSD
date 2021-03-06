﻿namespace Common.Library.Html
{
    public interface IHtmlTable
    {
        int RowCount { get; }
        int GetColCount(int row);
        IHtmlCell this[int row, int col] { get; }
    }
}
