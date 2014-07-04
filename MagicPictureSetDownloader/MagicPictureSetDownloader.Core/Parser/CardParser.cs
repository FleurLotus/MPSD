using System;
using System.Collections.Generic;

namespace MagicPictureSetDownloader.Core
{
    class CardParser: IParser<CardInfo>
    {
        private const string Start = @"<!-- Card Details Table -->";
        private const string End = @"<!-- End Card Details Table -->";


        public IEnumerable<CardInfo> Parse(string text)
        {
            //ALERT: à revoir pour parser réellement le texte et récuperer les images
            text = InfoParser.ExtractContent(text, Start, End);
            throw new NotImplementedException();
        }
    }
}
