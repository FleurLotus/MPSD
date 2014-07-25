namespace MagicPictureSetDownloader.Interface
{
    public interface ICard
    {
        int Id { get; set; }
        string Name { get; set; }
        string Text { get; set; }
        string Power { get; set; }
        string Toughness { get; set; }
        string CastingCost { get; set; }
        int? Loyalty { get; set; }
        string Type { get; set; }
    }
}