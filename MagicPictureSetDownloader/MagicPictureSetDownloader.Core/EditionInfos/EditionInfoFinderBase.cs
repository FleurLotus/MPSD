namespace MagicPictureSetDownloader.Core.EditionInfos
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal abstract class EditionInfoFinderBase : IEditionFinder
    {
        protected readonly Func<string,string> GetHtml;
        protected IDictionary<string, string> Replace;


        protected EditionInfoFinderBase(Func<string, string> getHtml)
        {
            GetHtml = getHtml;
        }
        protected abstract IList<EditionIconInfo> Parse(string text);
        protected virtual void GetIconUrl(EditionIconInfo editionIconInfo)
        {
        }

        public EditionIconInfo Find(string url, string wantedEdition)
        {
            IList<EditionIconInfo> list = Parse(url);

            EditionIconInfo ret = Matching(list, wantedEdition);
            GetIconUrl(ret);
            return ret;
        }
        
        private EditionIconInfo Matching(IList<EditionIconInfo> editionIconPage, string wantedEdition)
        {
            if (string.IsNullOrWhiteSpace(wantedEdition) || editionIconPage == null)
                return null;


            string wantedName = wantedEdition.Trim();
            string correctedWantedName = TryCorrect(wantedName);

            foreach (EditionIconInfo editionIconInfo in editionIconPage)
            {
                string name = editionIconInfo.Name.Trim();

                if (string.Compare(name, wantedName, StringComparison.InvariantCultureIgnoreCase) == 0)
                    return editionIconInfo;


                string correctedName = TryCorrect(name);

                if (string.Compare(correctedName, correctedWantedName, StringComparison.InvariantCultureIgnoreCase) == 0)
                    return editionIconInfo;
            }
            return null;
        }

        private string TryCorrect(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return name;

            return Replace.Aggregate(name, (current, kv) => current.Replace(kv.Key, kv.Value)).Trim();
        }


    }
}
