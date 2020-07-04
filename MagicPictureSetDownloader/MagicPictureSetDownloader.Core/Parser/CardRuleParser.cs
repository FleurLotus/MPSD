namespace MagicPictureSetDownloader.Core
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Xml;
    using Common.Library.Extension;

    using MagicPictureSetDownloader.Core.CardInfo;

    internal class CardRuleParser : CardParserBase, IParser<CardRuleInfo>
    {
        //Use class instead of id
        //ctl00_ctl00_ctl00_MainContent_SubContent_SubContent_rulingsContainer for normal card
        //ctl00_ctl00_ctl00_MainContent_SubContent_SubContent_ctl02_rulingsContainer for part A of multi part card
        //ctl00_ctl00_ctl00_MainContent_SubContent_SubContent_ctl03_rulingsContainer for part B of multi part card
        //private const string KeyStart = "ctl00_ctl00_ctl00_maincontent_subcontent_subcontent";
        //private const string KeyEnd = "rulingsContainer";

        public IEnumerable<CardRuleInfo> Parse(string text)
        {
            string[] cutTexts = ExtractCardText(text);

            return cutTexts.SelectMany(GetRules);
        }

        private IEnumerable<CardRuleInfo> GetRules(string text)
        {
            //Parsing
            using (XmlTextReader xmlReader = new XmlTextReader(new StringReader(SpecialXMLCorrection(text))))
            {
                //Go to ruling div
                while (xmlReader.Read())
                {
                    if (xmlReader.NodeType == XmlNodeType.Element)
                    {
                        string classValue = xmlReader.GetAttribute("class");
                        if (!string.IsNullOrEmpty(classValue) && classValue.ToLowerInvariant() == "postcontainer")
                        {
                            //Work on table
                            IAwareXmlTextReader reader = new AwareXmlTextReader(xmlReader);
                            while (reader.Read())
                            {
                                if (reader.NodeType == XmlNodeType.Element && reader.Name == "tr")
                                {
                                    string trClassValue = reader.GetAttribute("class");
                                    if (!string.IsNullOrEmpty(trClassValue))
                                    {
                                        trClassValue = trClassValue.ToLowerInvariant();
                                        if (trClassValue == "post evenitem" || trClassValue == "post odditem")
                                        {
                                            CardRuleInfo cardRuleInfo = WorkOnRow(new AwareXmlTextReader(reader));
                                            if (cardRuleInfo != null)
                                            {
                                                yield return cardRuleInfo;
                                            }
                                        }
                                    }
                                }
                            }

                            yield break;
                        }
                    }
                }
            }
        }

        private CardRuleInfo WorkOnRow(IAwareXmlTextReader xmlReader)
        {
            DateTime date = new DateTime();
            string rule = null;

            while (xmlReader.Read())
            {
                if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == "td")
                {
                    string tdIdValue = xmlReader.GetAttribute("id");
                    if (!string.IsNullOrEmpty(tdIdValue))
                    {
                        tdIdValue = tdIdValue.ToLowerInvariant();

                        string text = WorkOnTextBox(new AwareXmlTextReader(xmlReader));
                        if (tdIdValue.EndsWith("rulingdate"))
                        {
                            DateTime.TryParseExact(text, "M/d/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out date);
                        }
                        else if (tdIdValue.EndsWith("rulingtext"))
                        {
                            rule = text;
                        }
                    }
                }
            }

            if (rule == null || date == new DateTime())
            {
                throw new ParserException("Can't retrieve all data needed for rule");
            }

            if (string.IsNullOrWhiteSpace(rule))
            {
                //The rule text is retrieve but empty, ignore the rule
                return null;
            }

            return new CardRuleInfo { Date = date, Text = rule };
        }
        private string WorkOnTextBox(IAwareXmlTextReader xmlReader)
        {
            string value = string.Empty;
            while (xmlReader.Read())
            {
                string text = null;
                if (xmlReader.NodeType == XmlNodeType.Text)
                {
                    text = xmlReader.Value.HtmlTrim();
                }
                else if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == "img")
                {
                    text = SymbolParser.Parse(xmlReader);
                }

                if (!string.IsNullOrWhiteSpace(text))
                {
                    value += " " + text;
                }
            }

            return value.HtmlTrim();
        }
    }
}
