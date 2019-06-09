namespace MagicPictureSetDownloader.Core.Deck
{
    using System;
    using System.Net;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Collections.Generic;

    using MagicPictureSetDownloader.Interface;
    using MagicPictureSetDownloader.Db;

    public class PreconstructedDeckImporter
    {
        private const string BaseUrl = @"http://mtg.wtf";
        private readonly Regex _decksRegex = new Regex(@"<a href=""(?<url>/deck/\w+/[^>]+)"">", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private readonly Regex _deckNameRegex = new Regex(@"<h4>(?<name>[^<]+)</h4>", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private readonly Regex _deckEditionRegex = new Regex(@"<a download=""true"" href=""/deck/(?<edition>\w+)/[^>]*"">", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private readonly Regex _cardInfoRegex = new Regex(@"<a href=""/card/(?<edition>\w+)/[^>]*"">(?<name>.+)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private const string CardSplitter = @"<div class='card_entry'>"; 

        private IMagicDatabaseReadOnly MagicDatabase = MagicDatabaseManager.ReadOnly;

        public string GetRootUrl()
        {
            return string.Format("{0}/deck", BaseUrl);
        }

        internal string[] GetDeckUrls(string html)
        {
            if (string.IsNullOrWhiteSpace(html))
            {
                return new string[0];
            }
            
            string htmltext = WebUtility.HtmlDecode(html);
            MatchCollection matches = _decksRegex.Matches(htmltext);

            return matches.OfType<Match>().Select(m => BaseUrl + m.Groups["url"].Value).ToArray();
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
            if(!m.Success)
            {
                throw new ParserException("Could not find edition");
            }

            if (string.IsNullOrWhiteSpace(deckName))
            {
                return null;
            }

            IEdition deckEdition = GetEdition(deckName, m);
            if (deckEdition == null)
            {
                throw new ParserException("Could not find edition with code " + deckEdition);
            }
            if (MagicDatabase.GetPreconstructedDeck(deckEdition.Id, deckName) != null)
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
                        
                        ICard card = GetCard(m);
                        IEdition edition = GetEdition(deckName, m);

                        int idGatherer = MagicDatabase.GetIdGatherer(card, edition);
                        if (idGatherer == 0)
                        {
                            throw new ParserException(string.Format("Could not find card with idCard {0} and idEdition {1}", card.Id, edition.Id));

                        }

                        cards.Add(new DeckCardInfo(idGatherer, number));
                        break;
                    }
                }
            }

            if (cards.Count == 0)
            {
                return null;
            }
            DeckInfo deckInfo =  new DeckInfo(deckEdition.Id, deckName, cards);
            int cardCount = deckInfo.Count;
            //  60 Usual
            //  75 Usual with side board
            // 100 Commander
            //  30 or 26 Welcome Pack
            //  80 Archenemy
            if (cardCount != 60 && cardCount != 75 && cardCount != 100 &&
                cardCount != 30 && cardCount != 26 && cardCount != 80)
            {
                throw new ParserException(string.Format("Deck {0} countains {1} cards", deckName, cardCount));
            }

            return deckInfo;
        }

        private ICard GetCard(Match m)
        {
            string cardName = m.Groups["name"].Value.TrimEnd();
            string cardName2 = null;
  
            if (cardName.Contains("/"))
            {
                string[] c = cardName.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                cardName = c[0].Trim();
                cardName2 = c[1].Trim();
            }

            ICard card = MagicDatabase.GetCard(cardName, cardName2);
            if (card == null)
            {
                card = MagicDatabase.GetCard(cardName, null);
                if (card == null)
                {
                    throw new ParserException("Could not find Card with name " + cardName);
                }
            }

            return card;
        }

        private IEdition GetEdition(string deckName, Match m)
        {
            string cardEdition = m.Groups["edition"].Value.TrimEnd();
            //Archenemy
            if (cardEdition == "oe01")
            {
                cardEdition = "e01";
            }

            IEdition edition = MagicDatabase.GetEditionFromCode(cardEdition);
            if (edition == null)
            {
                //Special case for guild pack
                string cardEdition2 = cardEdition + "_" + deckName;
                if (cardEdition2.Length > 10)
                {
                    cardEdition2 = cardEdition2.Substring(0, 10);
                }
                edition = MagicDatabase.GetEditionFromCode(cardEdition2);
             }

            return edition;
        }
    }
}
