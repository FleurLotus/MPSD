namespace MagicPictureSetDownloader.Core
{
    internal static class CardNameParser
    {
        private const string Start = @"<span id=""ctl00_ctl00_ctl00_MainContent_SubContent_SubContentHeader_subtitleDisplay"">";
        private const string End = @"</span>";

        public static string Parse(string text)
        {
            string newtext = Parser.ExtractContent(text, Start, End, true, false);


            if (string.IsNullOrWhiteSpace(newtext) || newtext.Length>100)
                throw new ParserException("Error while parsing, can't retrieve ");

            return newtext.Replace(@" // ", @"//");
        }
    }
}
