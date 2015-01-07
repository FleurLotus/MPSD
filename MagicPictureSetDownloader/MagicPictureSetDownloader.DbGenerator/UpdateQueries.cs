
namespace MagicPictureSetDownloader.DbGenerator
{
    internal static class UpdateQueries
    {
        #region CreateLanguageTable
        internal const string CreateLanguageTable =
            @"
CREATE TABLE [Language] (
  [Id] int NOT NULL
, [Name] nvarchar(50) NOT NULL
, [AlternativeName] nvarchar(1000) NULL
);
GO
ALTER TABLE [Language] ADD CONSTRAINT [PK_Language] PRIMARY KEY ([Id]);
GO
INSERT INTO [Language] ([Id],[Name]) VALUES (0,N'Unknown');
GO
INSERT INTO [Language] ([Id],[Name]) VALUES (1,N'English');
GO
INSERT INTO [Language] ([Id],[Name]) VALUES (2,N'Italian');
GO
INSERT INTO [Language] ([Id],[Name]) VALUES (3,N'French');
GO
INSERT INTO [Language] ([Id],[Name]) VALUES (4,N'German');
GO
INSERT INTO [Language] ([Id],[Name], [AlternativeName]) VALUES (5,N'Portuguese', N'Portuguese (Brazil)');
GO
INSERT INTO [Language] ([Id],[Name]) VALUES (6,N'Spanish');
GO
INSERT INTO [Language] ([Id],[Name]) VALUES (7,N'Japanese');
GO
INSERT INTO [Language] ([Id],[Name]) VALUES (8,N'Korean');
GO
INSERT INTO [Language] ([Id],[Name]) VALUES (9,N'Chinese Traditional');
GO
INSERT INTO [Language] ([Id],[Name]) VALUES (10,N'Chinese Simplified');
GO
INSERT INTO [Language] ([Id],[Name]) VALUES (11,N'Russian');
GO";
        #endregion

        #region CreateTranslateTable
        internal const string CreateTranslateTable =
            @"
CREATE TABLE [Translate] (
  [IdCard] int NOT NULL
, [IdLanguage] int NOT NULL
, [Name] nvarchar(100) NOT NULL
);
GO
ALTER TABLE [Translate] ADD CONSTRAINT [PK_Translate] PRIMARY KEY ([IdCard], [IdLanguage]);
GO";
        #endregion

        #region RemoveCompleteSetQuery
        internal const string RemoveCompleteSetQuery =
            @"
UPDATE [Edition] SET [Completed] = 0;
GO";
        #endregion

        #region AddColumnLanguageToCardEditionsInCollectionQuery
        internal const string AddColumnLanguageToCardEditionsInCollectionQuery =
            @"
ALTER TABLE [CardEditionsInCollection] ADD [IdLanguage] int DEFAULT 0 NOT NULL;
GO";
        #endregion

        #region UpdateAlaraBlockReleaseDateQuery
        internal const string UpdateAlaraBlockReleaseDateQuery =
            @"
UPDATE Edition 
SET [ReleaseDate] = '20081003'
WHERE  [GathererName] = 'Shards of Alara'
GO
UPDATE Edition 
SET [ReleaseDate] = '20090206'
WHERE  [GathererName] = 'Conflux'
GO
UPDATE Edition 
SET [ReleaseDate] = '20090430'
WHERE  [GathererName] = 'Alara Reborn'
GO";
        #endregion

        #region InsertPromoRarity
        internal const string InsertPromoRarity =
            @"
INSERT INTO [Rarity] ([Name],[Code]) VALUES ('Promo', 'P')
GO";
        #endregion

        #region ExtendCardCastingCostLength
        internal const string ExtendCardCastingCostLength =
            @"
ALTER TABLE [Card] ALTER COLUMN [CastingCost] nvarchar(100) NULL
GO";
        #endregion        
        
        #region ExtendCardNameLength
        internal const string ExtendCardNameLength =
            @"
ALTER TABLE [Card] ALTER COLUMN [Name] nvarchar(150) NOT NULL
GO
ALTER TABLE [Card] ALTER COLUMN [PartName] nvarchar(150) NOT NULL
GO
ALTER TABLE [Card] ALTER COLUMN [OtherPartName] nvarchar(150) NULL
GO";
        #endregion

        #region ExtendTranslateNameLength
        internal const string ExtendTranslateNameLength =
            @"
ALTER TABLE [Translate] ALTER COLUMN [Name] nvarchar(150) NOT NULL
GO";
        #endregion
    }
}
