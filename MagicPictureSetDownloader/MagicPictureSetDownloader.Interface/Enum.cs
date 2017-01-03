namespace MagicPictureSetDownloader.Interface
{
    public enum TypeOfOption
    {
        Hierarchy,
        SelectedCollection,
        Display,
        Input,
        Upgrade,
    }

    public enum ExportFormat
    {
        MPSD = -1,
        MTGM = 0,
    }

    public enum ImportOption
    {
        NewCollection,
        AddToCollection,
    }
    public enum ExportImagesOption
    {
        OneByGathererId,
        OneByCardName,
    }
}
