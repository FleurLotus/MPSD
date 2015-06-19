namespace Common.Libray.Extension
{
    using System;
    using System.Linq;
    using System.Text;

    public static class StringExtension
    {
        private static readonly string[] _formatTags = { 
                                                           "<p>", "</p>",
                                                           "<i>", "</i>",
                                                           "<b>", "</b>",
                                                           "<strong>", "</strong>",
                                                           "<em>", "</em>",
                                                           "<small>", "</small>",
                                                           "<mark>", "</mark>",
                                                           "<del>", "</del>",
                                                           "<ins>", "</ins>",
                                                           "<sub>", "</sub>",
                                                           "<sup>", "</sup>",
                                                           "<h1>", "</h1>",
                                                           "<h2>", "</h2>",
                                                           "<h3>", "</h3>",
                                                           "<h4>", "</h4>",
                                                           "<h5>", "</h5>",
                                                           "<h6>", "</h6>"
                                                       };


        public static string Replace(this string str, string oldValue, string newValue, StringComparison comparison)
        {
            if (string.IsNullOrEmpty(str) || oldValue == null)
                return str;

            int index = str.IndexOf(oldValue, comparison);

            if (index == -1)
                return str;

            StringBuilder sb = new StringBuilder();

            int previousIndex = 0;

            while (index != -1)
            {
                sb.Append(str.Substring(previousIndex, index - previousIndex));
                sb.Append(newValue);
                index += oldValue.Length;

                previousIndex = index;
                index = str.IndexOf(oldValue, index, comparison);
            }
            sb.Append(str.Substring(previousIndex));

            return sb.ToString();
        }
        public static string HtmlTrim(this string source)
        {
            if (source == null)
                return null;

            return source.Replace("&nbsp;", " ").Trim(new[] { ' ', '\t', '\n', '\r' });
        }
        public static string HtmlRemoveFormatTag(this string source)
        {
            if (source == null)
                return null;

            return _formatTags.Aggregate(source, (s, iter) => s.Replace(iter, string.Empty, StringComparison.InvariantCultureIgnoreCase)).HtmlTrim();
        }
    }
}
