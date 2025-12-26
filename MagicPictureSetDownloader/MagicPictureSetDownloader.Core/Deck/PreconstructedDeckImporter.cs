namespace MagicPictureSetDownloader.Core.Deck
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Text.RegularExpressions;

    using MagicPictureSetDownloader.Db;
    using MagicPictureSetDownloader.Interface;

    public class PreconstructedDeckImporter
    {
        private const string BaseUrl = @"http://mtg.wtf";
        private readonly Regex _decksRegex = new Regex(@"<a href=""(?<url>/deck/\w+/[^>]+)"">.*\n</a>\((?<type>[^,]+),\s+\d+\s+cards\)", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline);
        private readonly Regex _deckNameRegex = new Regex(@"<h4>(?<name>[^<]+)</h4>", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private readonly Regex _deckEditionRegex = new Regex(@"<a download=""true"" href=""/deck/(?<edition>\w+)/[^>]*"">", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private readonly Regex _cardInfoRegex = new Regex(@"<a href=""(?<url>/card/(?<edition>\w+)/[^>]*)"">(?<name>.+)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private const string CardSplitter = @"<div class='card_entry'>";
        // Magic Online Commander
        private readonly Regex _excludedRegex = new Regex(@"/deck/(?:td0)/", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private readonly IMagicDatabaseReadOnly MagicDatabase = MagicDatabaseManager.ReadOnly;

        public PreconstructedDeckImporter()
        {
        }

        public string GetRootUrl()
        {
            return string.Format("{0}/deck", BaseUrl);
        }

        private bool IgnoreDeckTypeEdition(string name)
        {
            string checkName = name?.ToLower();

            return checkName.Contains("alchemy") || checkName.Contains("online") || checkName.Contains("arena") || checkName.Contains("historic brawl") || checkName.Contains("mtgo");
        }

        internal string[] GetDeckUrls(string html)
        {
            if (string.IsNullOrWhiteSpace(html))
            {
                return Array.Empty<string>();
            }

            string htmltext = WebUtility.HtmlDecode(html);
            MatchCollection matches = _decksRegex.Matches(htmltext);
            ICollection<IPreconstructedDeck> decks = MagicDatabase.GetAllPreconstructedDecks();
            return matches.OfType<Match>()
                           .Where(m => !IgnoreDeckTypeEdition(m.Groups["type"].Value))
                           .Select(m => BaseUrl + m.Groups["url"].Value)
                           .Where(u => decks.All(d => d.Url != u) && !_excludedRegex.IsMatch(u))
                           .ToArray();
        }

        internal DeckInfo ParseDeckPage(string html)
        {
            string htmltext = WebUtility.HtmlDecode(html);
            Match m = _deckNameRegex.Match(htmltext);
            if (!m.Success)
            {
                throw new ParserException("Could not find Title");
            }

            string deckName = m.Groups["name"].Value;
            m = _deckEditionRegex.Match(htmltext);
            if (!m.Success)
            {
                throw new ParserException("Could not find edition");
            }

            if (string.IsNullOrWhiteSpace(deckName))
            {
                return null;
            }

            IEdition deckEdition = GetEdition(deckName, m);
            if (MagicDatabase.GetPreconstructedDeck(deckEdition?.Id, deckName) != null)
            {
                return null;
            }

            //Split in block
            IList<DeckCardInfo> cards = new List<DeckCardInfo>();
            string[] tokens = htmltext.Split(new string[] { CardSplitter }, StringSplitOptions.RemoveEmptyEntries);

            //Start at 1 because cards start by CardSplitter
            for (int i = 1; i < tokens.Length; i++)
            {
                string[] lines = tokens[i].Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

                if (!int.TryParse(lines[0], out int number))
                {
                    throw new ParserException("Could not find Number");
                }

                foreach (string line in lines)
                {
                    m = _cardInfoRegex.Match(line);
                    if (m.Success)
                    {
                        string idScryFall = null;
                        string cardName = m.Groups["name"].Value.TrimEnd();

                        IEdition edition = GetEdition(deckName, m) ?? throw new ParserException($"Could not find edition for card in {deckName}");
                        ICard card = MagicDatabase.GetCard(cardName);
                        if (card != null)
                        {
                            idScryFall = MagicDatabase.GetIdScryFall(card, edition);
                            // Fallback for card special with double identical face
                            if (string.IsNullOrEmpty(idScryFall))
                            {
                                card = MagicDatabase.GetCard($"{cardName} // {cardName}");
                                if (card != null)
                                {
                                    idScryFall = MagicDatabase.GetIdScryFall(card, edition);
                                }
                            }
                        }
                        else
                        {
                            //Flavor name search 1st
                            idScryFall = MagicDatabase.GetIdScryFallByFlavorName(cardName, edition);
                            if (string.IsNullOrEmpty(idScryFall))
                            {
                                if (cardName.Contains("("))
                                {
                                    //Flavor name search 2nd
                                    idScryFall = MagicDatabase.GetIdScryFallByFlavorName(cardName[..cardName.LastIndexOf("(")].TrimEnd(), edition);
                                    if (string.IsNullOrEmpty(idScryFall))
                                    {
                                        // Try to get the real name between parenthesis
                                        string name = cardName[(cardName.LastIndexOf("(") + 1)..cardName.LastIndexOf(")")];
                                        card = MagicDatabase.GetCard(name);
                                        idScryFall = MagicDatabase.GetIdScryFall(card, edition);

                                        if (string.IsNullOrEmpty(idScryFall))
                                        {
                                            card = MagicDatabase.GetCard($"{name} // {name}");
                                            idScryFall = MagicDatabase.GetIdScryFall(card, edition);
                                        }
                                    }
                                }
                            }
                        }

                        if (string.IsNullOrEmpty(idScryFall))
                        {
                            throw new ParserException(string.Format("Could not find card with Card {0} and idEdition {1}", card?.Id.ToString() ?? cardName, edition.Id));
                        }
                        else
                        {
                            cards.Add(new DeckCardInfo(idScryFall, number));
                            break;
                        }
                    }
                }
            }

            if (cards.Count == 0)
            {
                return null;
            }
            return new DeckInfo(deckEdition?.Id, deckName, cards);
        }

        private IEdition GetEdition(string deckName, Match m)
        {
            string cardEdition = m.Groups["edition"].Value.TrimEnd();

            IEdition edition = MagicDatabase.GetEditionFromCode(cardEdition);
            if (edition == null)
            {
                //Special case for guild pack
                string cardEdition2 = $"{cardEdition}_{deckName}";
                if (cardEdition2.Length > 10)
                {
                    cardEdition2 = cardEdition2[..10];
                }
                edition = MagicDatabase.GetEditionFromCode(cardEdition2);
            }

            return edition;
        }
    }
}