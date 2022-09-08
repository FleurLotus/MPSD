namespace MagicPictureSetDownloader.Core
{
    using System;
    using System.Collections.Generic;
    using System.Text;

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
        public const string VariationsKey = "Variations";

        private static readonly IDictionary<string, string> _missingLoyalty = new Dictionary<string, string>
        {
            // Core Set 2021
            {@"Teferi, Master of Time</div>", "3" },
            // Jumpstart: Historic Horizons
            {@"Sarkhan, Wanderer to Shiv</div>", "4" },
            {@"Davriel, Soul Broker</div>", "4" },
            {@"Freyalise, Skyshroud Partisan</div>", "4" },
            {@"Kiora, the Tide's Fury</div>", "4" },
            {@"Teyo, Aegis Adept</div>", "4" },
            // Innistrad: Midnight Hunt Alchemy
            {@"Garruk, Wrath of the Wilds</div>", "3" },
            {@"Tibalt, Wicked Tormentor</div>", "3" },
            // Alchemy Horizons: Baldur's Gate
            {@"Tasha, Unholy Archmage</div>", "4" },
        };

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
                        tmpString = tmpString[..(endpos + LastCardEndData.Length)];
                    }
                }

                cardsText.Add(tmpString);
            }

            return cardsText.ToArray();
        }

        protected string SpecialXMLCorrection(string text)
        {
            StringBuilder sb = new StringBuilder();

            //Ensure unique root element because of fragment file
            sb.Append("<root>").Append(text).Append("</root>");

            //For XmlTextReader which doesn't support & caracter
            sb.Replace("&nbsp;", " ");
            sb.Replace("&lt;i&gt;", "<i>");
            sb.Replace("&lt;/i&gt;", "</i>");
            sb.Replace("&amp;", "&");
            sb.Replace("&", "&amp;");
            sb.Replace("<</i>", "&lt;</i>");
            sb.Replace("<</div>", "&lt;</div>");

            //Known XML issues in file
            sb.Replace("<div>", "<tr>");
            sb.Replace("</div\r\n", "</div>");
            sb.Replace("<i>", "");
            sb.Replace("</i>", "");
            sb.Replace("--->", "-->");
            
            return CardSpecificCorrection(sb.ToString());
        }

        private string CardSpecificCorrection(string text)
        {
            foreach (var kv in _missingLoyalty)
            {
                if (text.Contains(kv.Key))
                {
                    //Missing the loyauty
                    const string Start = @"Loyalty:</div>";
                    const string End = @"</div>";
                    const string Middle = @"<div class=""value"">";

                    int index = text.IndexOf(Start);
                    if (index >= 0)
                    {
                        int endIndex = text.IndexOf(End, index + Start.Length);
                        if (endIndex >= 0)
                        {
                            string toreplace = text.Substring(index, endIndex - index + End.Length);
                            int subIndex = toreplace.IndexOf(Middle);
                            if (subIndex >= 0)
                            {
                                string tocheck = toreplace.Substring(subIndex + Middle.Length, toreplace.Length - subIndex - Middle.Length - End.Length);
                                if (string.IsNullOrWhiteSpace(tocheck))
                                {
                                    //Confirm no  loyauty
                                    return text.Replace(toreplace, toreplace[..^End.Length] + kv.Value + End);
                                }
                            }
                            //string tocheck = text.Substring(index + 14, endIndex - index - 14);
                        }
                    }
                }
            }

            return text;
        }
    }
}
