namespace Common.Web
{
    using System;

    public static class UrlHelper
    {
        public static string ToAbsoluteUrl(string baseurl, string relativeurl, bool useOnlyDomain = false)
        {
            if (string.IsNullOrWhiteSpace(relativeurl))
            {
                return baseurl;
            }

            if (string.IsNullOrWhiteSpace(baseurl))
            {
                return relativeurl;
            }

            if (relativeurl.Contains("//"))
            {
                return relativeurl;
            }

            if (useOnlyDomain)
            {
                baseurl = ExtractBaseUrl(baseurl);
            }

            if (!baseurl.EndsWith("/"))
            {
                if (baseurl.Contains('/'))
                {
                    baseurl = baseurl[..(baseurl.LastIndexOf("/", StringComparison.InvariantCulture) + 1)];
                }
                else
                {
                    baseurl += "/";
                }
            }

            if (relativeurl.StartsWith("/"))
            {
                relativeurl = relativeurl[1..];
            }

            return RemovePathBack(baseurl + relativeurl);
        }

        private static string RemovePathBack(string url)
        {
            const string pathBack = "/../";
            int index;
            int start;
            while ((index = url.IndexOf(pathBack, StringComparison.InvariantCultureIgnoreCase)) >= 0)
            {
                if (index == 0)
                {
                    start = -1;
                }
                else
                {
                    start = url.LastIndexOf('/', index - 1);
                }
                if (start > 0 && url[start - 1] == ':')
                {
                    start++;
                }

                url = url[..(start + 1)] + url[(index + pathBack.Length)..];

                if (start < 0 && url.StartsWith("../"))
                {
                    url = "/" + url;
                }
            }
            return url;
        }
        private static string ExtractBaseUrl(string url)
        {
            const string postfixProtocol = @"://";

            int startIndex = url.IndexOf(postfixProtocol, StringComparison.InvariantCulture);
            if (startIndex < 0)
            {
                return url;
            }

            int index = url.IndexOf('/', startIndex + postfixProtocol.Length);
            if (index < 0)
            {
                return url;
            }

            return url[..(index + 1)];
        }
    }
}
