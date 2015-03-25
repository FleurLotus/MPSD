namespace MagicPictureSetDownloader.Db
{
    using System;
    using System.Data.SqlServerCe;
    using System.Linq;

    using Common.Database;
    using Common.Libray.Threading;

    using MagicPictureSetDownloader.Db.DAO;
    using MagicPictureSetDownloader.Interface;

    internal partial class MagicDatabase
    {
        public void UpdateEdition(IEdition iedition, string sourceName, string name, bool hasFoil, string code, int? idBlock, int? blockPosition, int? cardNumber, DateTime? releaseDate)
        {
            using (new WriterLock(_lock))
            {
                Edition edition = iedition as Edition;
                if (edition == null || string.IsNullOrWhiteSpace(sourceName) || string.IsNullOrWhiteSpace(name))
                    return;

                name = name.Trim();
                sourceName = sourceName.Trim();

                if (_editions.FirstOrDefault(e => edition.Id != e.Id && string.Compare(e.GathererName, sourceName, StringComparison.InvariantCultureIgnoreCase) == 0) != null)
                    return;

                //No need to update referencial because instance is still the same
                edition.Name = name;
                edition.GathererName = sourceName;
                edition.HasFoil = hasFoil;
                edition.Code = code;
                edition.IdBlock = idBlock;
                edition.BlockPosition =  idBlock.HasValue ? blockPosition : null;
                edition.CardNumber = cardNumber;
                edition.ReleaseDate = releaseDate;

                using (SqlCeConnection cnx = new SqlCeConnection(_connectionString))
                {
                    cnx.Open();
                    Mapper<Edition>.UpdateOne(cnx, edition);
                }
            }
        }
        public void UpdateBlock(IBlock iblock, string blockName)
        {
            using (new WriterLock(_lock))
            {
                Block block = iblock as Block;
                if (block == null ||string.IsNullOrWhiteSpace(blockName))
                    return;

                blockName = blockName.Trim();
                if (_blocks.Values.FirstOrDefault(b => string.Compare(b.Name, blockName, StringComparison.InvariantCultureIgnoreCase) == 0) != null)
                    return;

                //No need to update referencial because id do not change
                block.Name = blockName;

                using (SqlCeConnection cnx = new SqlCeConnection(_connectionString))
                {
                    cnx.Open();
                    Mapper<Block>.UpdateOne(cnx, block);
                }
            }
        }
        public void UpdateLanguage(ILanguage ilanguage, string languageName, string alternativeName)
        {
            using (new WriterLock(_lock))
            {
                Language language = ilanguage as Language;
                if (language == null || string.IsNullOrWhiteSpace(languageName))
                    return;

                languageName = languageName.Trim();
                if (alternativeName != null)
                    alternativeName = alternativeName.Trim();

                if (_languages.Values.FirstOrDefault(b => b.Id != language.Id && string.Compare(b.Name, languageName, StringComparison.InvariantCultureIgnoreCase) == 0) != null)
                    return;

                RemoveFromReferential(language);

                language.Name = languageName;
                language.AlternativeName = alternativeName;

                InsertInReferential(language);

                using (SqlCeConnection cnx = new SqlCeConnection(_connectionString))
                {
                    cnx.Open();
                    Mapper<Language>.UpdateOne(cnx, language);
                }
            }
        }
    }
}