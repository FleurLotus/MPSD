namespace MagicPictureSetDownloader.DbGenerator
{
    internal static class UpdateQueries
    {
        //
        public const string UpdateCastingCostForUltimateNightmare = 
@"UPDATE Card 
    SET CastingCost = '@X @Y @Z @R @R' 
  WHERE NAME = 'The Ultimate Nightmare of Wizards of the Coast® Customer Service' 
  AND CastingCost = '@X @R @R'";

        public const string UpdateCommander2015Code = 
@"UPDATE Edition 
    SET Code = 'C15' 
  WHERE NAME = 'Commander 2015'";

        public const string UpdateEditionAlternativeCode =
@"UPDATE EDITION 
   SET AlternativeCode = CASE WHEN AlternativeCode IS NULL THEN '{0}'
                         ELSE AlternativeCode || ';{0}'
                         END
  WHERE Name = '{1}'
  AND NOT IFNULL(AlternativeCode,'') LIKE '%{0}%'";

        public const string UpdateForceEditionAlternativeCode =
@"UPDATE EDITION 
   SET AlternativeCode = '{0}'
  WHERE Name = '{1}'";

        public const string UpdateEditionWithSpecialCard =
@"UPDATE EDITION 
   SET Completed = 0 
 WHERE NOT Id IN (SELECT IdEdition 
                  FROM CardEdition ce
                  INNER JOIN Card c ON c.Id = ce.IdCard
                  WHERE (c.Type IN ('Conspiracy', 'Scheme') OR c.Type like 'Plane %'))
AND GathererName in ('Archenemy', 'Magic: The Gathering—Conspiracy', 'Planechase', 'Planechase 2012 Edition')";
        public const string UpdateEditionWithNoCard =
@"UPDATE EDITION 
   SET Completed = 0 
  WHERE NOT Id IN (SELECT IdEdition FROM CardEdition)
  AND Name <> 'Vanguard'";

        public const string CopyImage =
@"INSERT INTO TreePicture
  SELECT '{1}', Image
  FROM TreePicture t1
  WHERE t1.Name = '{0}'
  AND NOT EXISTS(SELECT 1 
                 FROM TreePicture
                 WHERE Name = '{1}')";
    }

}
