namespace MagicPictureSetDownloader.DbGenerator
{
    internal static class UpdateQueries
    {
        public const string InsertNewTreePicture = @"INSERT INTO TreePicture VALUES (@name , @value)";
        public const string SelectTreePicture = @"SELECT Name, Image FROM TreePicture";

        public const string CreateRulingTable =
@"CREATE TABLE [Ruling] (
  [Id] INTEGER PRIMARY KEY NOT NULL 
, [AddDate] TEXT NOT NULL
, [IdCard] INTEGER NOT NULL
, [Text] TEXT NOT NULL
)";

      }
}
