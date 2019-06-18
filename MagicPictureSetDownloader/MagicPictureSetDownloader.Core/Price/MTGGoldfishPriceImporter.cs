namespace MagicPictureSetDownloader.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using MagicPictureSetDownloader.Db;
    using MagicPictureSetDownloader.Interface;

    internal class MTGGoldfishPriceImporter : TableParserBase, IPriceImporter
    {
        private const string BasePriceUrl = @"https://www.mtggoldfish.com/index/{0}#paper";
        private const string BaseFoilPriceUrl = @"https://www.mtggoldfish.com/index/{0}_F#paper";

        private const string Start = @"tablesorter-bootstrap-popover-paper'>";
        private const string End = @"<div class='index-price-table-online'>";

        private static readonly Regex _titleRegex = new Regex(@"<title>.*</title>", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex _cardNameRegex = new Regex(@"<a[^>]*>(?<name>.*?)(\s+\(.+\))?</a>", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex _dataRegex = new Regex(@"<td[^>]*>(?<value>.*)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private readonly string[] _excluded = new[] {
            "The Monarch", //Token of conspiracy 
        };

        private readonly IDictionary<string, string> _mapping = new Dictionary<string, string>
            {
                //Seventh Edition
                {"7ED", "7E" }, 
                //Apocalypse
                {"APC", "AP" },
                //Exodus
                {"EXO", "EX" },
                //Invasion
                {"INV", "IN" },
                //Mirage
                {"MIR", "MI" },
                //Mercadian Masques
                {"MMQ", "MM" },
                //Masterpiece Series: Kaladesh Inventions
                {"MPS", "MS2" },
                //Masterpiece Series: Amonkhet Invocations
                {"MPS_AKH", "MS3" },
                //Nemesis
                {"NEM", "NE" },
                //Odyssey
                {"ODY", "OD" },
                //Planechase
                {"HOP", "PC1" },
                //Prophecy
                {"PCY", "PR" },
                //Planeshift
                {"PLS", "PS" },
                //Stronghold
                {"STH", "ST" },
                //Tempest
                {"TMP", "TE" },
                //Urza's Destiny
                {"UDS", "UD" },
                //Urza's Legacy
                {"ULG", "UL" },
                //Urza's Saga
                {"USG", "UZ" },
                //Visions
                {"VIS", "VI" },
                //Weatherlight
                {"WTH", "WL" },

                //Do not exist on Website
                //Global Series: Jiang Yanggu and Mu Yanling
                {"GS1", "" },
                //Starter 2000
                {"S00", "" },
                //Starter 1999
                {"S99", "" },
                //Signature Spellbook: Jace
                {"SS1", "" },
                //Welcome Deck 2016
                {"W16", "" },
                //Welcome Deck 2017
                {"W17", "" },
                //Guilds of Ravnica Intro pack
                {"GK1_BOROS", "" },
                {"GK1_DIMIR", "" },
                {"GK1_GOLGAR", "" },
                {"GK1_IZZET", "" },
                {"GK1_SELESN", "" },
               //Guilds of Ravnica Allegiance pack
                {"GK2_GRUUL", "" },
                {"GK2_AZORIU", "" },
                {"GK2_ORZHOV", "" },
                {"GK2_RAKDOS", "" },
                {"GK2_SIMIC", "" },
                //Guild of Ravnica Mythic Edition
                {"MPS_GRN", "" },
                //Ravnica Allegiance Mythic Edition
                {"MPS_RNA", "" },
                //War of the Spark Mythic Edition
                {"MPS_WAR", "" },
                 //Ultimate Master Toppers
                {"UMA_BOX", "" },
                //Game Night
                {"GNT", "" },
                //Gift Pack
                {"G18", "" },
                //DD Anthology
                {"DVD", "" },
                {"GVL", "" },
                {"JVC", "" },
                {"EVG", "" },
                {"DD1", "" },
                 //Special Set
                {"CP1", "" },
                {"CP2", "" },
                {"CP3", "" },
                {"DPA", "" },
                {"DKM", "" },
                {"ATH", "" },
                {"ITP", "" },
                {"TD2", "" },
        };
        private readonly IMagicDatabaseReadOnly _magicDatabase = MagicDatabaseManager.ReadOnly;

        public PriceSource PriceSource
        {
            get { return PriceSource.MTGGoldfish; }
        }

        public IEnumerable<PriceInfo> Parse(string text)
        {
            bool isFoil = IsFoil(text);
            return GetPrice(text, isFoil);
        }
        public string[] GetUrls()
        {
            ICollection<IBlock> blocks = _magicDatabase.GetAllBlocks();
            int idBlockFun = blocks.Where(b => b.Name.IndexOf("Fun", StringComparison.InvariantCultureIgnoreCase) >= 0).Select(b => b.Id).First();
            int idBlockOnlineOnly = blocks.Where(b => b.Name.IndexOf("OnlineOnly", StringComparison.InvariantCultureIgnoreCase) >= 0).Select(b => b.Id).First();


            IList<string> urls = new List<string>();

            foreach (IEdition edition in _magicDatabase.GetAllEditions()
                                                .Where(ed => !ed.IdBlock.HasValue || (ed.IdBlock.Value != idBlockFun && ed.IdBlock.Value != idBlockOnlineOnly))
                                                .Ordered())
            {
                string code = GetWebSiteCode(edition.Code);
                if (!string.IsNullOrWhiteSpace(code))
                {
                    urls.Add(string.Format(BasePriceUrl, code));
                    if (edition.HasFoil)
                    {
                        urls.Add(string.Format(BaseFoilPriceUrl, code));
                    }
                }
            }

            return urls.ToArray();
        }
        private string GetWebSiteCode(string editionCode)
        {
            if (!string.IsNullOrWhiteSpace(editionCode) && _mapping.TryGetValue(editionCode, out string webSiteCode))
            {
                return webSiteCode;
            }

            return editionCode;
        }
        private string GetEditionCodeFromWebSite(string webSiteEditionCode)
        {
            if (!string.IsNullOrWhiteSpace(webSiteEditionCode))
            {
                foreach (KeyValuePair<string, string> kv in _mapping)
                {
                    if (kv.Value == webSiteEditionCode)
                    {
                        return kv.Key;
                    }
                }
            }
            return webSiteEditionCode;
        }
        private bool IsFoil(string text)
        {
            Match m = _titleRegex.Match(text);
            if (!m.Success)
            {
                throw new ParserException("Can't get title");
            }

            return !string.IsNullOrWhiteSpace(m.Value) && m.Value.ToUpper().Contains("FOILS");
        }

        private IEnumerable<PriceInfo> GetPrice(string text, bool isFoil)
        {
            string cutText = Parser.ExtractContent(text, Start, End, true, false);

            bool header = true;
            int priceIndex = -1;
            int cardNameIndex = -1;
            int editionCodeIndex = -1;
            IEdition edition = null;

            foreach (string row in GetTableRow(cutText))
            {
                string trimedrow = row.Replace("\r", "").Replace("\n", "");

                string[] columns = trimedrow.Split(new[] { "</td>", "</th>" }, StringSplitOptions.None);
                if (header)
                {
                    for (int i = 0; i < columns.Length; i++)
                    {
                        string column = columns[i];
                        if (column.Contains(">Card"))
                        {
                            cardNameIndex = i;
                        }
                        else if (column.Contains(">Price"))
                        {
                            priceIndex = i;
                        }
                        else if (column.Contains(">Set"))
                        {
                            editionCodeIndex = i;
                        }
                    }

                    if (cardNameIndex == -1 || priceIndex == -1 || editionCodeIndex == -1)
                    {
                        throw new ParserException("Can't parse price page.");
                    }

                    header = false;
                    continue;
                }
                //Edition
                Match m = _dataRegex.Match(columns[editionCodeIndex]);
                if (!m.Success)
                {
                    throw new ParserException("Can't find edition code");
                }

                string editionCode = GetEditionCodeFromWebSite(m.Groups["value"].Value.Trim());
                if (string.IsNullOrWhiteSpace(editionCode))
                {
                    throw new ParserException("Edition code is null or empty");
                }
                if (edition == null || edition.Code != editionCode)
                {
                   edition = _magicDatabase.GetEditionFromCode(editionCode);
                }
                if (edition == null)
                {
                    throw new ParserException("Unknown edition : " + editionCode);
                }

                //Card
                m = _cardNameRegex.Match(columns[cardNameIndex]);
                if (!m.Success)
                {
                    throw new ParserException("Can't find card name");
                }

                string cardName = m.Groups["name"].Value.Trim();
                if (string.IsNullOrWhiteSpace(cardName))
                {
                    throw new ParserException("Card name is null or empty");
                }

                ICard card = _magicDatabase.GetCard(cardName, null);
                if (card == null)
                {
                    if (_excluded.Contains(cardName))
                    {
                        continue;
                    }

                     throw new ParserException("Unknown card : " + cardName);
                }

                int idGatherer = _magicDatabase.GetIdGatherer(card, edition);
                if (idGatherer == 0)
                {
                    throw new ParserException(string.Format("Can't find gatherer id for card {0} edition {1}", card, edition));
                }

                //Price
                m = _dataRegex.Match(columns[priceIndex]);
                if (!m.Success)
                {
                    throw new ParserException("Can't find price");
                }

                string priceString = m.Groups["value"].Value.Trim();
                if (string.IsNullOrWhiteSpace(priceString))
                {
                    throw new ParserException("Price is null or empty");
                }

                if (!int.TryParse(priceString.Replace(".", string.Empty).Replace(",", string.Empty), out int price))
                {
                    throw new ParserException("Price is not a number");
                }

                //TODO: check if price null is a failure or not
                yield return new PriceInfo { Foil = isFoil, Value = price, IdGatherer = idGatherer };
            }
        }
    }
}
