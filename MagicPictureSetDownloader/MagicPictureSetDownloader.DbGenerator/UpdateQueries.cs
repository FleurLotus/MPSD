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
WHERE Code = 'HVM'";

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

        public const string UpdateKaladeshInventionCode =
@"UPDATE Edition 
SET Code = 'MPS'
WHERE GathererName = 'Masterpiece Series: Kaladesh Inventions'";

        public const string DeleteKaladeshInventionGathererIdChange =
@"DELETE FROM CardEdition 
WHERE IdGatherer IN (417582, 417640, 417669, 417685,417745) 
AND IdEdition = (SELECT id FROM Edition WHERE Name='Kaladesh Inventions')";

        public const string UpdateKaladeshInventionMissingCard =
    @"UPDATE Edition 
SET Completed = 0, CardNumber = 54
WHERE GathererName = 'Masterpiece Series: Kaladesh Inventions' 
AND  (SELECT COUNT(*) FROM CardEdition ce  INNER JOIN Edition e ON e.Id =ce.IdEdition WHERE  GathererName = 'Masterpiece Series: Kaladesh Inventions') < 54";
    }
}
