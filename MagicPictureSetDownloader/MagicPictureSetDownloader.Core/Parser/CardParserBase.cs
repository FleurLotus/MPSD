namespace MagicPictureSetDownloader.Core
{
    using System;
    using System.Collections.Generic;

    internal class CardParserBase
    {
        private const string Start = @"<div class=""contentcontainer"">";
        private const string End = @"<div class=""clear""></div>";
        private const string CardStartBlock = @"<td id=""ctl00_ctl00_ctl00_MainContent_SubContent_SubContent_cardComponent";
        private const string CardStartData = @"<table class=""cardDetails";
        private const string LastCardEndData = @"</td>";
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
            //This a way to get rid for header and footer of HTML file
            string cutText = Parser.ExtractContent(text, Start, End, false, true);
            
            string[] tokens = cutText.Split(new[] { CardStartBlock }, StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Length == 1)
            {
                throw new ParserException("Wrong parsing of card block extraction! Check HTML source");
            }
            //tokens[0] before card ignored
            //tokens[1+] real cards
            //last token contains also after card data
            for (int i = 1; i < tokens.Length; i++)
            {
                string tmpString = CardStartBlock + tokens[i];
                int index = tmpString.LastIndexOf(CardStartData, StringComparison.InvariantCulture);
                if (index < 0)
                {
                    continue;
                }

                if (i == tokens.Length - 1)
                {
                    int endpos = tmpString.LastIndexOf(LastCardEndData, StringComparison.InvariantCulture);
                    if (endpos >= 0)
                    {
                        tmpString = tmpString.Substring(0, endpos + LastCardEndData.Length);
                    }
                }

                cardsText.Add(tmpString);
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
