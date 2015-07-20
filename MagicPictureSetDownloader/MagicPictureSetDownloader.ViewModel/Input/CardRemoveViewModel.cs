namespace MagicPictureSetDownloader.ViewModel.Input
{
    using MagicPictureSetDownloader.Interface;

    public class CardRemoveViewModel : UpdateViewModelCommun
    {
        public CardRemoveViewModel(string collectionName, ICard card)
            : base(collectionName)
        {
            Source = new CardSourceViewModel(MagicDatabase, SourceCollection, card);

            Display.Title = "Remove card";
        }
        public CardSourceViewModel Source { get; private set; }
        
        protected override bool OkCommandCanExecute(object o)
        {
            return Source.Count > 0 && Source.Count <= Source.MaxCount && Source.EditionSelected != null;
        }
    }
}
