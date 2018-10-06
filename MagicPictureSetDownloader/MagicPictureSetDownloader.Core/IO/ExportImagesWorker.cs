namespace MagicPictureSetDownloader.Core.IO
{
    using System;

    using System.IO;
    using System.Text.RegularExpressions;
    using Common.Drawing;
    using MagicPictureSetDownloader.Db;
    using MagicPictureSetDownloader.Interface;

    public class ExportImagesWorker
    {
        private readonly IMagicDatabaseReadOnly MagicDatabase = MagicDatabaseManager.ReadOnly;

        public ICardAllDbInfo[] GetAllCardWithPicture()
        {
            return MagicDatabase.GetCardsWithPicture();
        }
        public void Export(ICardAllDbInfo cardInfo, string outpath, string suffix, ExportImagesOption exportImageOption)
        {
            if (!Directory.Exists(outpath))
            {
                throw new ArgumentException("output path doesn't exist", "outpath");
            }

            string folder = outpath;
            if (exportImageOption == ExportImagesOption.OneByGathererId)
            {
                //Windows doesn't allow to create a folder with the name CON
                folder = string.IsNullOrEmpty(cardInfo.Edition.Code) || cardInfo.Edition.Code == "CON" ? cardInfo.Edition.Name : cardInfo.Edition.Code;
                folder = Path.Combine(outpath, folder);
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }
            }

            Save(folder, suffix, cardInfo.IdGatherer, cardInfo.Card.Name);

            if (cardInfo.IdGathererPart2 > 0)
            {
                Save(folder, suffix, cardInfo.IdGathererPart2, cardInfo.CardPart2.Name);
            }
        }

        private void Save(string folder, string suffix, int idGatherer, string cardName)
        {
            IPicture picture = MagicDatabase.GetPicture(idGatherer, true);
            if (picture == null)
            {
                return;
            }

            byte[] img = picture.Image;
            cardName = Regex.Replace(cardName, @"[/:""\?]", string.Empty);
            string imagePath = Path.Combine(folder, string.Format("{0}{1}{2}", cardName, suffix, img.ToImage().GetFileExtension()));

            if (File.Exists(imagePath))
            {
                return;
            }

            using (FileStream fs = new FileStream(imagePath, FileMode.CreateNew))
            {
                fs.Write(img, 0, img.Length);
            }
        }
    }
}
