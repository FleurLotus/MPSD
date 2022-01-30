namespace MagicPictureSetDownloader.DbGenerator
{
    internal static class UpdateQueries
    {
        public const string SelectPreconstuctedDeckCards =
@"SELECT pdce.IdGatherer, pd.Name, e.GathererName, pdce.Number
FROM PreconstructedDeckCardEdition pdce
INNER JOIN PreconstructedDeck pd ON pd.id = pdce.IdPreconstructedDeck
INNER JOIN Edition e ON e.id = pd.IdEdition";

        public const string InsertPreconstructedDeckCards =
@"INSERT INTO PreconstructedDeckCardEdition(IdGatherer,IdPreconstructedDeck,Number)
SELECT ce.IdGatherer, pd.Id, @number
FROM PreconstructedDeck pd
INNER JOIN Edition e ON e.id = pd.IdEdition
INNER JOIN CardEdition ce ON ce.IdGatherer = @idgatherer
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

        public const string SelectCardEditionVariation =
@"SELECT cev.IdGatherer, cev.OtherIdGatherer, cev.Url
FROM CardEditionVariation cev
";
        public const string InsertNewCardEditionVariation =
@"INSERT INTO CardEditionVariation(IdGatherer, OtherIdGatherer, Url)
SELECT IdGatherer, @otherIdGatherer, @url
FROM CardEdition
WHERE IdGatherer = @idGatherer";

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
WHERE Code = @code
";

        public const string CorrectHasFoilTrue =
@"UPDATE Edition
SET HasFoil = 1
WHERE Code = @code
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

        public const string AddNoneGathererSets =
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
        public const string CorrectVehicle =
@"
UPDATE Card
SET Power = @power, Toughness = @toughness
WHERE Name = @name AND Power IS NULL AND Toughness IS NULL
";
        public const string CorrectMystericBoosterText =
@"
UPDATE Card
SET Text = REPLACE(REPLACE(REPLACE(Text, '&lt;i&gt;', ''), '&lt;/i&gt;', ''), '&lt;i/&gt;', '')
WHERE Text like '%&lt;%'
";
        public const string AddAltArtColumnToCollection =
@"
ALTER TABLE CardEditionsInCollection
ADD COLUMN [AltArtNumber] INTEGER NOT NULL DEFAULT 0
";
        public const string AddFoilAltArtColumnToCollection =
@"
ALTER TABLE CardEditionsInCollection
ADD COLUMN [FoilAltArtNumber] INTEGER NOT NULL DEFAULT 0
";
        public const string AddIsAltArtColumnToAudit =
@"
ALTER TABLE Audit
ADD COLUMN [IsAltArt] INTEGER NULL
";
        public const string UpdateAltArtColumnOfAudit =
@"
UPDATE Audit
SET [IsAltArt] = 0
WHERE [IsAltArt] IS NULL AND [IsFoil] IS NOT NULL
";

        public const string CreateCardEditionVariationTable =
@"CREATE TABLE [CardEditionVariation] (
  [IdGatherer] INTEGER NOT NULL 
, [OtherIdGatherer] INTEGER NOT NULL 
, [Url]	TEXT NOT NULL
, PRIMARY KEY([IdGatherer], [OtherIdGatherer])
, FOREIGN KEY([IdGatherer]) REFERENCES `CardEdition`([IdGatherer])
)";

        public const string UpdateSecretLairDropMissingCard =
    @"UPDATE Edition 
SET Completed = 0, CardNumber = @value
WHERE GathererName = 'Secret Lair Drop' 
AND  (SELECT COUNT(*) FROM CardEdition ce  INNER JOIN Edition e ON e.Id = ce.IdEdition WHERE  GathererName = 'Secret Lair Drop') < @value";

        public const string CreateBackSideModalDoubleFacedCardTable =
    @"CREATE TABLE [BackSideModalDoubleFacedCard] (
  [Name] TEXT NOT NULL 
, PRIMARY KEY([Name])
)";

        public const string InsertBackSideModalDoubleFacedCard =
@"INSERT INTO BackSideModalDoubleFacedCard (Name)
SELECT @name
WHERE NOT EXISTS(SELECT 1 FROM BackSideModalDoubleFacedCard WHERE Name = @name)";

        public const string CorrectAECardName =
@"UPDATE Card
SET Name =  REPLACE(Name, 'Æ', 'Ae')
WHERE Name LIKE '%Æ%'";

        public const string CorrectAECardPartName =
@"UPDATE Card
SET PartName =  REPLACE(PartName, 'Æ', 'Ae')
WHERE PartName LIKE '%Æ%'";

        public const string CorrectKillDestroyCard =
@"UPDATE Card
SET PartName =  'Kill! Destroy!', Name = 'Kill! Destroy!'
WHERE Name = 'Kill Destroy'";
    }
}
