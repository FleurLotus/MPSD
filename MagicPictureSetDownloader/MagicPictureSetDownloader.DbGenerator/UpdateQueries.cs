namespace MagicPictureSetDownloader.DbGenerator
{
    internal static class UpdateQueries
    {
        public const string InsertNewTreePicture = @"INSERT INTO TreePicture VALUES (@name , @value)";
        public const string SelectTreePicture = @"SELECT Name, Image FROM TreePicture";


        public const string RemoveDuelDeckFromName =
@"UPDATE Edition 
SET NAME = SUBSTR(Name, 13)
WHERE Name like 'Duel Decks: %'";

        public const string UpdateCodeHeroesMonsterDeck =
@"UPDATE Edition
SET Code = 'DDL' 
WHERE Code='HVM'";

        public const string CreateRulingTable =
@"CREATE TABLE [Ruling] (
  [Id] INTEGER PRIMARY KEY NOT NULL 
, [AddDate] TEXT NOT NULL
, [IdCard] INTEGER NOT NULL
, [Text] TEXT NOT NULL
)";

        public const string UpdateKaladeshInventionGathererName =
@"UPDATE Edition 
SET GathererName = 'Masterpiece Series: Kaladesh Inventions'
WHERE GathererName = 'Kaladesh Inventions'";
      }
}
