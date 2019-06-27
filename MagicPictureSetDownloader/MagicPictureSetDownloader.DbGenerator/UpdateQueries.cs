namespace MagicPictureSetDownloader.DbGenerator
{
    internal static class UpdateQueries
    {
        public const string InsertNewTreePicture = @"INSERT INTO TreePicture VALUES (@name , @value)";
        public const string SelectTreePicture = @"SELECT Name, Image FROM TreePicture";

        public const string SelectPreconstuctedDeckCards =
@"SELECT pdce.IdGatherer, pd.Name, e.GathererName, pdce.Number
FROM PreconstructedDeckCardEdition pdce
INNER JOIN PreconstructedDeck pd ON pd.id = pdce.IdPreconstructedDeck
INNER JOIN Edition e ON e.id = pd.IdEdition";

        public const string InsertPreconstructedDeckCards =
@"INSERT INTO PreconstructedDeckCardEdition(IdGatherer,IdPreconstructedDeck,Number)
SELECT @idgatherer, pd.Id, @number
FROM PreconstructedDeck pd
INNER JOIN Edition e ON e.id = pd.IdEdition
WHERE e.GathererName = @gatherername
AND pd.Name = @name
";
        public const string SelectPreconstuctedDecks =
@"SELECT pd.Url, pd.Name, e.GathererName
FROM PreconstructedDeck pd
INNER JOIN Edition e ON e.Id = pd.IdEdition";

        public const string InsertNewPreconstuctedDecks =
@"INSERT INTO PreconstructedDeck(IdEdition, Name, Url)
SELECT Id, @name, @url
FROM Edition
WHERE GathererName = @gatherername";

        public const string SelectFakeIdGathererCardEdition =
@"SELECT ce.IdGatherer, e.GathererName, r.Code, c.Name, c.PartName, ce.Url
FROM CardEdition ce
INNER JOIN Edition e ON e.Id = ce.IdEdition
INNER JOIN Card c ON c.Id = ce.IdCard
INNER JOIN Rarity r ON r.Id = ce.IdRarity
WHERE IdGatherer < 0";

        public const string InsertFakeIdGathererCardEdition =
@"INSERT INTO CardEdition(IdGatherer, IdEdition, IdCard, IdRarity, Url)
SELECT @idgatherer, e.Id, c.Id, r.Id, @url
FROM Edition e
CROSS JOIN Rarity r
CROSS JOIN Card c
WHERE e.GathererName = @gatherername
AND r.Code = @raritycode
ANd c.Name = @cardname 
AND c.PartName = @cardpartname ";

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

        public static readonly string[] RemoveWrongCardFromGuildOfRavnica = {
@"DELETE FROM Translate
WHERE IdCard IN (SELECT Id 
                 FROM Card 
                 WHERE Name IN ('Boros Guildgate (b)', 'Dimir Guildgate (b)','Golgari Guildgate (b)', 'Izzet Guildgate (b)', 'Selesnya Guildgate (b)',
                                'Boros Guildgate (a)', 'Dimir Guildgate (a)','Golgari Guildgate (a)', 'Izzet Guildgate (a)', 'Selesnya Guildgate (a)')
                )",
@"DELETE FROM CardEdition
WHERE IdCard IN (SELECT Id 
                 FROM Card 
                 WHERE Name IN ('Boros Guildgate (b)', 'Dimir Guildgate (b)','Golgari Guildgate (b)', 'Izzet Guildgate (b)', 'Selesnya Guildgate (b)')
                )",
@"DELETE FROM Card
WHERE Name IN ('Boros Guildgate (b)', 'Dimir Guildgate (b)','Golgari Guildgate (b)', 'Izzet Guildgate (b)', 'Selesnya Guildgate (b)')",
@"UPDATE CardEdition 
SET IdCard = (SELECT Id FROM Card WHERE Name = 'Boros Guildgate')
WHERE IdCard = (SELECT Id FROM Card WHERE Name = 'Boros Guildgate (a)')",
@"UPDATE CardEdition 
SET IdCard = (SELECT Id FROM Card WHERE Name = 'Dimir Guildgate')
WHERE IdCard = (SELECT Id FROM Card WHERE Name = 'Dimir Guildgate (a)')",
@"UPDATE CardEdition 
SET IdCard = (SELECT Id FROM Card WHERE Name = 'Golgari Guildgate')
WHERE IdCard = (SELECT Id FROM Card WHERE Name = 'Golgari Guildgate (a)')",
@"UPDATE CardEdition 
SET IdCard = (SELECT Id FROM Card WHERE Name = 'Izzet Guildgate')
WHERE IdCard = (SELECT Id FROM Card WHERE Name = 'Izzet Guildgate (a)')",
@"UPDATE CardEdition 
SET IdCard = (SELECT Id FROM Card WHERE Name = 'Selesnya Guildgate')
WHERE IdCard = (SELECT Id FROM Card WHERE Name = 'Selesnya Guildgate (a)')",
@"DELETE FROM Card
WHERE Name IN ('Boros Guildgate (a)', 'Dimir Guildgate (a)','Golgari Guildgate (a)', 'Izzet Guildgate (a)', 'Selesnya Guildgate (a)')"
};

        public const string CreatePriceTable =
@"CREATE TABLE [Price] (
  [Id] INTEGER PRIMARY KEY NOT NULL 
, [AddDate] TEXT NOT NULL
, [Source] TEXT NOT NULL
, [IdGatherer] INTEGER NOT NULL
, [Foil] INTEGER NOT NULL
, [Value] INTEGER NOT NULL
)";

        public const string CorrectHasFoilFalse =
@"UPDATE Edition
SET HasFoil = 0
WHERE Code IN ('V15','V16','V17',
'C15','C16','CMA','C17','CM2','C18',
'DDO','DDP','DDQ','DDR','DDS','DDT','DDU','GS1',
'E01','PCA'
)";
        public const string CorrectHasFoilTrue =
@"UPDATE Edition
SET HasFoil = 1
WHERE Code IN ('CNS','EXO')
";
        public const string CorrectBattleBondPartnerNotFlipCard =
@"UPDATE Card
SET OtherPartName = NULL 
WHERE OtherPartName IS NOT NULL AND Id IN (SELECT IdCard FROM CardEdition WHERE IdGatherer between 445969 And 445990)
";

        public const string CreatePreconstructedDeckTable =
@"CREATE TABLE [PreconstructedDeck] (
  [Id] INTEGER PRIMARY KEY NOT NULL 
, [IdEdition] INTEGER NOT NULL 
, [Name] TEXT NOT NULL
, [Url] TEXT NULL
)";

        public const string CreatePreconstructedDeckCardEditionTable =
@"CREATE TABLE [PreconstructedDeckCardEdition] (
  [IdPreconstructedDeck] INTEGER NOT NULL 
, [IdGatherer] INTEGER NOT NULL 
, [Number] INTEGER NOT NULL
, FOREIGN KEY([IdGatherer]) REFERENCES `CardEdition`([IdGatherer])
, FOREIGN KEY([IdPreconstructedDeck]) REFERENCES [PreconstructedDeck]([Id])
)";

        public const string AddNoneGtahererSets =
@"
INSERT INTO Edition (Name, Code, GathererName, Completed, HasFoil)
SELECT @name, @code, 'MTG.WTF-' || @name, 0, 1
WHERE NOT EXISTS(SELECT 1 FROM Edition WHERE Name = @name)
";
        public const string AddAlternativeCode =
@"
UPDATE Edition
SET AlternativeCode = CASE WHEN AlternativeCode IS NULL THEN @code ELSE AlternativeCode || ';' || @code END
WHERE Name = @name AND NOT IFNULL(AlternativeCode,'') LIKE '%' || @code || '%'
";
    }
}
