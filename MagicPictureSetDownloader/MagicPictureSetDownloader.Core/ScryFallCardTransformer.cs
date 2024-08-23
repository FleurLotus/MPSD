namespace MagicPictureSetDownloader.Core
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
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

        public void Start()
        {
            var parserTasks = Enumerable.Range(0, 4).Select(_ => Task.Run((Action)Parse)).ToArray();
            var updateTasks = Enumerable.Range(0, 2).Select(_ => Task.Run((Action)Update)).ToArray();

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
                    //ALERT TO Create CardWithExtraInfo from scryfall Card so CardFace, Edition, CardCardFace, Card etc...

                    /*
                    Card
                        Name
                        Layout
                    => Id

                    CardFace
                            Name
                            Text
                            Power
                            Toughness
                            CastingCost
                            Loyalty
                            Defense
                            Type
                            Url
                            IdCard
                            IsMainFace

                    CardEdition
                                IdEdition
                                IdCard
                                IdRarity
                                IdScryFall

                    ExternalIds
                                CardIdSource
                                ExternalId
                    
                    CardEditionVariation ???

                    */
                    CardWithExtraInfo c = new CardWithExtraInfo();


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
                    //ALERT Insert/Update in db all the parse data 
                    //string pictureUrl = WebAccess.ToAbsoluteUrl(jobData.Url, cardWithExtraInfo.PictureUrl);
                    //int idGatherer = Parser.ExtractIdGatherer(pictureUrl);

                    //_downloadManager.InsertCardInDb(cardWithExtraInfo);
                    //_downloadManager.InsertCardFaceInDb(cardWithExtraInfo);
                    //_downloadManager.InsertCardEditionInDb(jobData.EditionId, cardWithExtraInfo);
                    //_downloadManager.InsertNewExternalIds();


                    //foreach (string otherIdScryFall in cardWithExtraInfo.OtherIdScryFall)
                    //{
                    //    _downloadManager.InsertCardEditionVariationInDb(idGatherer, otherIdScryFall, WebAccess.ToAbsoluteUrl(jobData.Url, string.Format(Parser.AlternativePictureUrl, otherIdScryFall), true));
                    //}

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
