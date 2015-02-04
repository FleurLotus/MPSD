
namespace MagicPictureSetDownloader.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal static class EditionPageMapper
    {
        private static readonly IDictionary<string, string> _replace = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
                                                                           {
                                                                               { "&", "and" },
                                                                               { "Sixth", "6th" },
                                                                               { "Seventh", "7th" },
                                                                               { "Eighth", "8th" },
                                                                               { "Ninth", "9th" },
                                                                               { "Tenth", "10th" },
                                                                               { "Ravnica: City of Guilds", "Ravnica" },
                                                                               { "Elves vs. Dragons", "Elves vs. Goblins" }, //bug in Gatherer Name
                                                                               { "Unlimited", "XXXX" },  //Do not get the wrong image
                                                                               { " Anthology,", ":" },

                                                                               { "(2014)", null },
                                                                               { "Magic: The Gathering", null },
                                                                               { "Limited Edition", null },
                                                                               { "Edition", null },
                                                                               { "Core Set", null },
                                                                               { "Classic", null },
                                                                               { "—", null },
                                                                               { "-", null },
                                                                               { "Magic", null },
                                                                           };

        public static EditionIconInfo Matching(this IEnumerable<EditionIconInfo> editionIconPage, string wantedEdition)
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

                if (editionIconInfo.CorrectedName == null)
                    editionIconInfo.CorrectedName = TryCorrect(name);

                if (string.Compare(editionIconInfo.CorrectedName, correctedWantedName, StringComparison.InvariantCultureIgnoreCase) == 0)
                    return editionIconInfo;
            }
            return null;
        }

        private static string TryCorrect(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return name;

            return _replace.Aggregate(name, (current, kv) => current.Replace(kv.Key, kv.Value)).Trim();
        }

    }
}
