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
WHERE e.Name = @editionName
AND pd.Name = @name
";
        public const string SelectPreconstuctedDecks =
@"SELECT pd.Url, pd.Name, e.Name AS EditionName
FROM PreconstructedDeck pd
INNER JOIN Edition e ON e.Id = pd.IdEdition";
        
    public const string InsertNewPreconstuctedDecks =
@"INSERT INTO PreconstructedDeck(IdEdition, Name, Url)
SELECT Id, @name, @url
FROM Edition
WHERE Name = @editionName";

    public const string SelectCardEditionVariation =
@"SELECT cev.IdScryFall, cev.OtherIdScryFall, cev.Url
FROM CardEditionVariation cev
";

        public const string InsertNewCardEditionVariation =
@"INSERT INTO CardEditionVariation(IdScryFall, OtherIdScryFall, Url)
SELECT IdScryFall, @otherIdScryFall, @url
FROM CardEdition
WHERE IdScryFall = @idScryFall";

    }
}
