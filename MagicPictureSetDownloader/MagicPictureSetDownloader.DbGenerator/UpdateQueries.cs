namespace MagicPictureSetDownloader.DbGenerator
{
    internal static class UpdateQueries
    {
        //
        public const string UpdateCastingCostForUltimateNightmare = "UPDATE Card SET CastingCost = '@X @Y @Z @R @R' WHERE NAME = 'The Ultimate Nightmare of Wizards of the Coast® Customer Service' AND  CastingCost = '@X @R @R'";
    }
}
