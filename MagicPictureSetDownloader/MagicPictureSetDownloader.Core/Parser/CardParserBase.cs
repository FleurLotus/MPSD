namespace MagicPictureSetDownloader.Core
{
    using System;
    using System.Collections.Generic;

    internal class CardParserBase
    {
        private const string Start = @"<!-- Card Details Table -->";
        private const string End = @"<!-- End Card Details Table -->";
        private const string SubCardStart = @"<table class=""cardDetails cardComponent""";

        public const string ImageKey = "Image";
        public const string NameKey = "Name";
        public const string ManaCostKey = "ManaCost";
        public const string CmcKey = "CMC";
        public const string TypeKey = "Type";
        public const string TextKey = "Text";
        public const string FlavorKey = "Flavor";
        public const string PTKey = "PT";
        public const string SetKey = "Set";
        public const string RarityKey = "Rarity";
        public const string NumberKey = "Number";
        public const string ArtistKey = "Artist";

        protected CardParserBase()
        {
        }

        protected string[] ExtractCardText(string text)
        {
            List<string> cardsText = new List<string>();

            //No decode because we need to do a special correction for XmlTextReader to be able to read HTML with javascript
            //To end because of multi part card
            string cutText = Parser.ExtractContent(text, Start, End, false, true);

            if (cutText.IndexOf(End, StringComparison.InvariantCulture) >= 0)
            {
                //Case for multi part card
                foreach (string subinfo in cutText.Split(new[] { End }, StringSplitOptions.RemoveEmptyEntries))
                {
                    int index = subinfo.LastIndexOf(SubCardStart, StringComparison.InvariantCulture);
                    if (index < 0)
                    {
                        continue;
                    }

                    cardsText.Add(subinfo.Substring(index));
                }

                if (cardsText.Count != 2)
                {
                    throw new ParserException("Wrong number of parsed cards in a single block of multipart card! Check HTML source");
                }
            }
            else
            {
                //Case for normal card
                cardsText.Add(cutText);
            }

            return cardsText.ToArray();
        }

        protected string SpecialXMLCorrection(string text)
        {
            //Ensure unique root element because of fragment file
            text = "<root>" + text + "</root>";

            //For XmlTextReader which doesn't support & caracter
            text = text.Replace("&nbsp;", " ");
            text = text.Replace("&amp;", "&");
            text = text.Replace("&", "&amp;");
            text = text.Replace("<</i>", "&lt;</i>");
            text = text.Replace("<</div>", "&lt;</div>");

            //Known XML issues in file
            text = text.Replace("<div>", "<tr>");
            text = text.Replace("</div\r\n", "</div>");
            text = text.Replace("<i>", "");
            text = text.Replace("</i>", "");
            text = text.Replace("--->", "-->");

            return text;
        }
    }
}
