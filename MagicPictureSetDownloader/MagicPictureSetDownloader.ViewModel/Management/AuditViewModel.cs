namespace MagicPictureSetDownloader.ViewModel.Management
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Common.Library.Collection;
    using Common.ViewModel.Dialog;

    using MagicPictureSetDownloader.Db;
    using MagicPictureSetDownloader.Interface;

    public class AuditViewModel : DialogViewModelBase
    {
        private DateTime _minDate;
        private DateTime _maxDate;
        private readonly IAudit[] _allAudit;
        private readonly IMagicDatabaseReadOnly _magicDatabase;
        
        public AuditViewModel()
        {
            _magicDatabase = MagicDatabaseManager.ReadOnly;
            _allAudit = _magicDatabase.GetAllAudits().ToArray();

            Display.Title = "Audit";
            Display.OkCommandLabel = "Load";
            Display.CancelCommandLabel = "Close";

            MinDate = DateTime.UtcNow.Date.AddDays(-1);
            MaxDate = DateTime.UtcNow.Date;
            AuditInfos = new RangeObservableCollection<AuditInfo>();
        }

        public DateTime MaxDate
        {
            get { return _maxDate; }
            set
            {
                if (value != _maxDate)
                {
                    _maxDate = value;
                    if (MinDate > MaxDate)
                        MinDate = MaxDate;
                    OnNotifyPropertyChanged(() => MaxDate);
                }
            }
        }
        public DateTime MinDate
        {
            get { return _minDate; }
            set
            {
                if (value != _minDate)
                {
                    _minDate = value;
                    if (MinDate > MaxDate)
                        MaxDate = MinDate;
                    OnNotifyPropertyChanged(() => MinDate);
                }
            }
        }
        public RangeObservableCollection<AuditInfo> AuditInfos { get; private set; }

        protected override void OkCommandExecute(object o)
        {
            AuditInfos.Clear();
            AuditInfos.AddRange(GetAudit());
        }

        private IEnumerable<AuditInfo> GetAudit()
        {
            foreach (IAudit audit in _allAudit.Where(a => a.OperationDate >= MinDate && a.OperationDate < MaxDate.AddDays(1)))
            {
                AuditInfo info = new AuditInfo { Quantity = audit.Quantity, OperationDate = audit.OperationDate.ToLocalTime().ToString("G"), IsFoil = audit.IsFoil.HasValue && audit.IsFoil.Value };
                
                ICardCollection cardCollection = _magicDatabase.GetCollection(audit.IdCollection);
                info.CollectionName = cardCollection == null ? "(Deleted) " + audit.IdCollection : cardCollection.Name;

                if (audit.IdGatherer.HasValue)
                {
                    ICard card = _magicDatabase.GetCard(audit.IdGatherer.Value);
                    if (card == null)
                    {
                        info.CardName = "(Not found) " + audit.IdGatherer.Value;
                        info.EditionName = "(Not found) " + audit.IdGatherer.Value;
                    }
                    else
                    {
                        info.CardName = card.Name;
                        IEdition edition = _magicDatabase.GetEdition(audit.IdGatherer.Value);
                        info.EditionName = edition == null ? "(Not found) " + audit.IdGatherer.Value : edition.Name;
                    }
                    if (audit.IdLanguage.HasValue)
                    {
                        ILanguage language = _magicDatabase.GetLanguage(audit.IdLanguage.Value);
                        info.Language = language == null ? "(Not found) " + audit.IdLanguage.Value : language.Name;
                    }
                    else
                    {
                        info.Language = "(Missing language)";
                    }
                }

                yield return info;
            }
        }
        protected override bool OkCommandCanExecute(object o)
        {
            return MinDate < MaxDate && MinDate < DateTime.UtcNow.Date;
        }
    }
}
