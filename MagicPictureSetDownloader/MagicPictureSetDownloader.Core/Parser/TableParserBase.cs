namespace MagicPictureSetDownloader.Core
{
    using System;
    using System.Collections.Generic;

    internal abstract class TableParserBase
    {
        protected IEnumerable<string> GetTableRow(string newtext)
        {
            int pos = 0;
            while (pos >= 0)
            {
                pos = newtext.IndexOf("<tr", pos, StringComparison.OrdinalIgnoreCase);
                if (pos == -1)
                {
                    yield break;
                }

                int end = newtext.IndexOf("</tr", pos, StringComparison.OrdinalIgnoreCase);
                if (end == -1 || end <= pos)
                {
                    yield break;
                }

                yield return newtext.Substring(pos, end - pos);

                pos = end;
            }
        }
    }
}
