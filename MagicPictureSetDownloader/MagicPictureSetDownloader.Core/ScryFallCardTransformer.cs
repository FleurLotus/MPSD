namespace MagicPictureSetDownloader.Core
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;

    using Common.Library.Notify;
    using MagicPictureSetDownloader.Interface;
    using MagicPictureSetDownloader.ScryFall.JsonLite;

    public class ScryFallCardTransformer
    {
        public event EventHandler<EventArgs<string>> Error;
        public event EventHandler Finished;

        private readonly DownloadManager _downloadManager;
        private readonly IProgressReporter _progressReporter;
        private volatile bool _isStopping;

        private readonly BlockingCollection<Card> _inputs = new BlockingCollection<Card>();
        private readonly BlockingCollection<CardWithExtraInfo> _parsedInput = new BlockingCollection<CardWithExtraInfo>(100);

        public ScryFallCardTransformer(DownloadManager downloadManager, IProgressReporter progressReporter)
        {
            _downloadManager = downloadManager;
            _progressReporter = progressReporter;
        }

        public void AddRange(IEnumerable<Card> cards)
        {
            foreach (Card card in cards)
            {
                _inputs.Add(card);
            }
        }

        public void StartLanguage()
        {
            _inputs.CompleteAdding();
            foreach (Card card in _inputs.GetConsumingEnumerable())
            {
                try
                {
                    if (_isStopping)
                    {
                        return;
                    }

                    CardWithExtraInfo c = new CardWithExtraInfo
                    {
                        Name = card.Name,
                        IdScryFall = card.Id.ToString(),
                        Edition = card.Set?.ToUpper(),
                        Layout = card.Layout.ToString(),
                        Rarity = card.Rarity.ToString(),
                        Language = card.Language.ToString(),
                        PrintedName = card.PrintedName,
                    };
    
                    _downloadManager.InsertLanguageInDb(c);

                    _progressReporter.Progress();
                }
                catch (Exception ex)
                {
                    SendError(ex, $"{card.Name}");
                }
            }

            _progressReporter.Finish();
            OnFinished();
        }

        public void Start()
        {
            var parserTasks = Enumerable.Range(0, 1).Select(_ => Task.Run((Action)Parse)).ToArray();
            var updateTasks = Enumerable.Range(0, 1).Select(_ => Task.Run((Action)Update)).ToArray();

            _inputs.CompleteAdding();
            Task.WaitAll(parserTasks);
            _parsedInput.CompleteAdding();
            Task.WaitAll(updateTasks);

            _progressReporter.Finish();
            OnFinished();
        }

        private void Parse()
        {
            foreach (Card card in _inputs.GetConsumingEnumerable())
            {
                try
                {
                    if (_isStopping)
                    {
                        return;
                    }

                    CardWithExtraInfo c = new CardWithExtraInfo
                    {
                        Name = card.Name,
                        IdScryFall = card.Id.ToString(),
                        Edition = card.Set?.ToUpper(),
                        Layout = card.Layout.ToString(),
                        Rarity = card.Rarity.ToString(),
                        Language = card.Language.ToString(),
                        PrintedName = card.PrintedName,
                    };

                    if (card.MtgoId.HasValue)
                    {
                        c.ExternalId.Add((CardIdSource.Mtgo, card.MtgoId.Value.ToString()));
                    }
                    if (card.MtgoFoilId.HasValue)
                    {
                        c.ExternalId.Add((CardIdSource.MtgoFoil, card.MtgoFoilId.Value.ToString()));
                    }
                    if (card.TcgplayerId.HasValue)
                    {
                        c.ExternalId.Add((CardIdSource.Tcgplayer, card.TcgplayerId.Value.ToString()));
                    }
                    if (card.TcgplayerEtchedId.HasValue)
                    {
                        c.ExternalId.Add((CardIdSource.TcgplayerEtched, card.TcgplayerEtchedId.Value.ToString()));
                    }
                    if (card.CardmarketId.HasValue)
                    {
                        c.ExternalId.Add((CardIdSource.Cardmarket, card.CardmarketId.Value.ToString()));
                    }
                    if (card.MultiverseIds == null || card.MultiverseIds.Count> 0)
                    {
                        foreach (int id in card.MultiverseIds)
                        {
                            c.ExternalId.Add((CardIdSource.Multiverse, id.ToString()));
                        }
                    }

                    //Basic Card
                    if (card.CardFaces == null || card.CardFaces.Count == 0)
                    {
                        CardFaceWithExtraInfo cf = new CardFaceWithExtraInfo
                        {
                            Name = card.Name,
                            Text = card.OracleText,
                            Power = card.Power,
                            Toughness = card.Toughness,
                            CastingCost = card.ManaCost,
                            Loyalty = card.Loyalty,
                            Defense = card.Defense,
                            Type = card.TypeLine,
                            PictureUrl = card.ImageUris?.Normal.ToString(),
                            IsMainFace = true,
                        };
                        c.CardFaceWithExtraInfos.Add(cf);
                    }
                    else if (card.CardFaces.Count == 2)
                    {
                        CardFace cardFace = card.CardFaces[0];

                        Uri image = cardFace.ImageUris?.Normal ?? card.ImageUris?.Normal;

                        CardFaceWithExtraInfo cf = new CardFaceWithExtraInfo
                        {
                            Name = cardFace.Name,
                            Text = cardFace.OracleText,
                            Power = cardFace.Power,
                            Toughness = cardFace.Toughness,
                            CastingCost = cardFace.ManaCost,
                            Loyalty = cardFace.Loyalty,
                            Defense = cardFace.Defense,
                            Type = cardFace.TypeLine,
                            PictureUrl = image.ToString(),
                            IsMainFace = true,
                        };
                        c.CardFaceWithExtraInfos.Add(cf);

                        cardFace = card.CardFaces[1];
                        image = cardFace.ImageUris?.Normal ?? card.ImageUris?.Normal;

                        cf = new CardFaceWithExtraInfo
                        {
                            Name = cardFace.Name,
                            Text = cardFace.OracleText,
                            Power = cardFace.Power,
                            Toughness = cardFace.Toughness,
                            CastingCost = cardFace.ManaCost,
                            Loyalty = cardFace.Loyalty,
                            Defense = cardFace.Defense,
                            Type = cardFace.TypeLine,
                            PictureUrl = image.ToString(),
                            IsMainFace = false,
                        };
                        c.CardFaceWithExtraInfos.Add(cf);
                    }
                    else  //Card of multiple cardface like "Who // What // When // Where // Why"
                    {
                        CardFaceWithExtraInfo cf = new CardFaceWithExtraInfo
                        {
                            Name = card.Name,
                            Power = card.Power,
                            Toughness = card.Toughness,
                            CastingCost = card.ManaCost,
                            Loyalty = card.Loyalty,
                            Defense = card.Defense,
                            Type = card.TypeLine,
                            PictureUrl = card.ImageUris?.Normal.ToString(),
                            IsMainFace = true,
                        };

                        for (int i = 0; i< card.CardFaces.Count;i++)
                        {
                            if (i > 0)
                            {
                                cf.Text += "\n-----\n";
                            }

                            CardFace cardFace = card.CardFaces[i];
                            cf.Text += $"{cardFace.Name}\n{cardFace.ManaCost}\n{cardFace.TypeLine}\n{cardFace.OracleText}";
                        }
                        c.CardFaceWithExtraInfos.Add(cf);
                    }

                    _parsedInput.Add(c);
                }
                catch (Exception ex)
                {
                    SendError(ex, $"{card.Name}");
                }
            }
        }
        private void Update()
        {
            foreach (CardWithExtraInfo cardWithExtraInfo in _parsedInput.GetConsumingEnumerable())
            {
                try
                {
                    if (_isStopping)
                    {
                        return;
                    }
                    _downloadManager.InsertCardInDb(cardWithExtraInfo);

                    _progressReporter.Progress();
                }
                catch (Exception ex)
                {
                    SendError(ex, cardWithExtraInfo.Name);
                }
            }
        }
        public void Stop()
        {
            _isStopping = true;
        }
        private void SendError(Exception ex, string url)
        {
            OnError($"{url} -> {ex.Message}");
        }
        private void OnError(string message)
        {
            var e = Error;
            if (e != null)
            {
                e(this, new EventArgs<string>(message));
            }
        }
        private void OnFinished()
        {
            var e = Finished;
            if (e != null)
            {
                e(this, EventArgs.Empty);
            }
        }
    }
}
