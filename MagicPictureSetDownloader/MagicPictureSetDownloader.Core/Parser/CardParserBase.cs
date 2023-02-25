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

        private static readonly IDictionary<string, IDictionary<string, string>> _missingProperty = new Dictionary<string, IDictionary<string, string>>
        {
            { "Loyalty:", new Dictionary<string, string>
                {
                    // Core Set 2021
                    {@"Teferi, Master of Time", "3" },
                    // Jumpstart: Historic Horizons
                    {@"Sarkhan, Wanderer to Shiv", "4" },
                    {@"Davriel, Soul Broker", "4" },
                    {@"Freyalise, Skyshroud Partisan", "4" },
                    {@"Kiora, the Tide's Fury", "4" },
                    {@"Teyo, Aegis Adept", "4" },
                    // Innistrad: Midnight Hunt Alchemy
                    {@"Garruk, Wrath of the Wilds", "3" },
                    {@"Tibalt, Wicked Tormentor", "3" },
                    // Alchemy Horizons: Baldur's Gate
                    {@"Tasha, Unholy Archmage", "4" },
                    // Dominaria United
                    {@"Ajani, Sleeper Agent", "4" },
                    // Phyrexia: All Will Be One
                    {@"Jace, the Perfected Mind", "5" },
                    {@"Kaito, Dancing Shadow", "3" },
                    {@"Kaya, Intangible Slayer", "6" },
                    {@"Koth, Fire of Resistance", "4" },
                    {@"Vraska, Betrayal's Sting", "6" },
                    {@"Lukka, Bound to Ruin", "5" },
                    {@"Nahiri, the Unforgiving", "5" },
                    {@"The Eternal Wanderer", "5" },
                    {@"Nissa, Ascended Animist", "7" },
                    {@"Tyvar, Jubilant Brawler", "3" },
                }
            },
            { "Rarity:", new Dictionary<string, string>
                {
                    //Warhammer 40,000 Commander
                    {"Fabricate", "Rare" }
                }
            },
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
            foreach (string property in _missingProperty.Keys)
            {
                foreach (var kv in _missingProperty[property])
                {
                    string key = kv.Key + "</div>";
                    if (text.Contains(key))
                    {
                        //Missing the property
                        string Start = property + "</div>";
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
                                        //Confirm no property
                                        return text.Replace(toreplace, toreplace[..^End.Length] + kv.Value + End);
                                    }
                                }
                                //string tocheck = text.Substring(index + 14, endIndex - index - 14);
                            }
                        }
                    }
                }
            }

            return text;
        }
    }
}
