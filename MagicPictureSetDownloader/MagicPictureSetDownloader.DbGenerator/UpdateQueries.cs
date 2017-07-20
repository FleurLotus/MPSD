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

        public const string DeleteKaladeshInventionGathererIdChange =
@"DELETE FROM CardEdition 
WHERE IdGatherer IN (417582, 417640, 417669, 417685,417745) 
AND IdEdition = (SELECT id FROM Edition WHERE Name='Kaladesh Inventions')";

        public const string UpdateKaladeshInventionMissingCard =
@"UPDATE Edition 
SET Completed = 0, CardNumber = 54
WHERE GathererName = 'Masterpiece Series: Kaladesh Inventions' 
AND  (SELECT COUNT(*) FROM CardEdition ce  INNER JOIN Edition e ON e.Id = ce.IdEdition WHERE  GathererName = 'Masterpiece Series: Kaladesh Inventions') < 54";

        public const string UpdateKaladeshInventionBlock =
@"UPDATE Edition 
SET IdBlock = (SELECT IdBlock FROM Edition WHERE GathererName = 'Kaladesh')
WHERE GathererName = 'Masterpiece Series: Kaladesh Inventions'";

        public const string UpdateEditionMissingCode =
@"UPDATE Edition 
SET Code = @code
WHERE GathererName = @name";

        public const string UpdateAmonkhetInvocationsMissingCard =
@"UPDATE Edition 
SET Completed = 0, CardNumber = 54
WHERE GathererName = 'Masterpiece Series: Amonkhet Invocations' 
AND  (SELECT COUNT(*) FROM CardEdition ce  INNER JOIN Edition e ON e.Id = ce.IdEdition WHERE  GathererName = 'Masterpiece Series: Amonkhet Invocations') < 54";

        public static readonly string[] ChangeCardLoyaltyColumnType = {
@"ALTER TABLE Card RENAME TO Temp_Card",
@"CREATE TABLE Card (Id INTEGER PRIMARY KEY NOT NULL,Name TEXT NOT NULL,Text TEXT,Power TEXT,Toughness TEXT,CastingCost TEXT,Loyalty TEXT DEFAULT (null),Type TEXT NOT NULL,PartName TEXT NOT NULL,OtherPartName TEXT)",
@"INSERT INTO Card SELECT Id,Name,Text,Power,Toughness,CastingCost,Loyalty,Type,PartName,OtherPartName FROM Temp_Card",
@"DROP TABLE Temp_Card",
    };

        public const string UpdateHourofDevastationReleaseDate =
@"UPDATE Edition
SET ReleaseDate = '2017-07-14 00:00:00'
WHERE GathererName = 'Hour of Devastation'";
    }
}
