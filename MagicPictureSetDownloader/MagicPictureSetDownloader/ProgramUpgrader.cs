namespace MagicPictureSetDownloader
{
    using System;
    using System.IO;
    using System.Xml;
    using System.Diagnostics;
    using System.Reflection;

    using MagicPictureSetDownloader.Core;

    //TODO: Create a auto upgrade program maybe using a repo TO BE CODED
    //TODO: how to manage error
    //TODO: should be in a ViewModel/Core
    internal static class ProgramUpgrader
    {
        private const string LastVersionUrl = @"https://www.dropbox.com/s/0p3e0rb8dpjml6a/LastVersion.xml?dl=1";

        internal static bool CheckNewVerion()
        {
            XmlDocument doc = new XmlDocument();
            //TODO: use webaccess
            doc.Load(LastVersionUrl);

            XmlNode lastVersionNode = doc.SelectSingleNode(@"Version/Last");
            if (lastVersionNode == null)
                return false;

            XmlAttribute versionNumberAttribute = lastVersionNode.Attributes["Number"];
            if (versionNumberAttribute == null)
                return false;

            string versionNumber = versionNumberAttribute.Value;
            if (string.IsNullOrWhiteSpace(versionNumber))
                return false;

            Version newVersion = new Version(versionNumber);

            Assembly entryAssembly = Assembly.GetEntryAssembly();
            Version currentVersion = entryAssembly.GetName().Version;

            if (currentVersion >= newVersion)
                return false;

            XmlNode urlNode = lastVersionNode.SelectSingleNode("Url");
            if (urlNode == null)
                return false;

            string newVersionFileUrl = urlNode.Value;
            if (string.IsNullOrWhiteSpace(newVersionFileUrl))
                return false;

            {
                // display user ask for upgrade
                //if yes download file and display progress
                //unzip in temp and put path in temporyDirectory
                string temporyDirectory = null;
                if (!Directory.Exists(temporyDirectory))
                    throw new DirectoryNotFoundException("Can't upgrade, unzipped directory not found");

                //From http://www.codeproject.com/Articles/31454/How-To-Make-Your-Application-Delete-Itself-Immedia
                ProcessStartInfo Info = new ProcessStartInfo();
                Info.Arguments = string.Format("/C choice /C Y /N /D Y /T 3 & copy /Y \"{0}\" \"{1}\"", Path.Combine(temporyDirectory, "*.*"), Path.GetDirectoryName(entryAssembly.Location));
                Info.WindowStyle = ProcessWindowStyle.Hidden;
                Info.CreateNoWindow = true;
                Info.FileName = "cmd.exe";
                Process.Start(Info);
                return true;
            }

            return false;
        }
    }
}
