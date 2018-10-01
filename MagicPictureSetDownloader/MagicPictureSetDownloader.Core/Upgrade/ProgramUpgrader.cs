namespace MagicPictureSetDownloader.Core.Upgrade
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using System.Xml;

    using Common.Web;
    using Common.Zip;

    public enum UpgradeStatus
    {
        NotChecked,
        UpToDate,
        NeedToBeUpdated,
        CantCheck,
    }
    
    public class ProgramUpgrader
    {
        private const string LastVersionUrl = @"https://www.dropbox.com/s/0p3e0rb8dpjml6a/LastVersion.xml?dl=1";

        private readonly WebAccess _webaccess = new WebAccess();

        public ProgramUpgrader()
        {
            Status = UpgradeStatus.NotChecked;
        }

        public UpgradeStatus Status { get; private set; }
        public string NewVersionComment { get; private set; }
        public string NewVersionUrl { get; private set; }
        public Version NewVersionNumberVersion { get; private set; }

        private void GetInfo()
        {
            NewVersionNumberVersion = null;
            NewVersionUrl = null;
            NewVersionComment = null;

            XmlNode lastVersionNode = GetNewVersionFile().SelectSingleNode(@"Version/Last");
            if (lastVersionNode == null)
            {
                return;
            }

            XmlAttribute versionNumberAttribute = lastVersionNode.Attributes["Number"];
            if (versionNumberAttribute == null)
            {
                return;
            }

            string newVersionNumber = versionNumberAttribute.Value;
            if (string.IsNullOrWhiteSpace(newVersionNumber))
            {
                return;
            }

            XmlNode urlNode = lastVersionNode.SelectSingleNode("Url");
            if (urlNode == null)
            {
                return;
            }

            string newVersionUrl = urlNode.InnerText;
            if (string.IsNullOrWhiteSpace(newVersionUrl))
            {
                return;
            }

            XmlNode commentNode = lastVersionNode.SelectSingleNode("Comment");
            if (commentNode != null)
            {
                string newVersionComment = commentNode.InnerText;
                if (string.IsNullOrWhiteSpace(newVersionComment))
                {
                    NewVersionComment = newVersionComment;
                }
            }

            NewVersionNumberVersion = new Version(newVersionNumber);
            NewVersionUrl = newVersionUrl;
        }
        private XmlDocument GetNewVersionFile()
        {
            byte[] array = _webaccess.GetFile(LastVersionUrl);
            string xml = Encoding.UTF8.GetString(array);

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            return doc;
        }
        public bool HasNewVersionAvailable()
        {
            try
            {
                GetInfo();

                if (NewVersionNumberVersion == null)
                {
                    throw new ProgramUpgraderException("Can't get info from new version file");
                }

                Assembly entryAssembly = Assembly.GetEntryAssembly();
                Version currentVersion = entryAssembly.GetName().Version;

                bool hasNewVersion = currentVersion < NewVersionNumberVersion;
                Status = hasNewVersion ? UpgradeStatus.NeedToBeUpdated : UpgradeStatus.UpToDate;
                return hasNewVersion;
            }
            catch (ProgramUpgraderException)
            {
                Status = UpgradeStatus.CantCheck;
                throw;
            }
            catch (Exception ex)
            {
                Status = UpgradeStatus.CantCheck;
                throw new ProgramUpgraderException("Can't check", ex);
            }
        }

        public void Upgrade()
        {
            if (NewVersionUrl == null)
            {
                throw new ProgramUpgraderException("Can't get info from new version file");
            }

            if (!HasNewVersionAvailable())
            {
                throw new ProgramUpgraderException("No call of upgrade if HasNewVersionAvailable is false");
            }

            string temporyDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString().ToUpperInvariant());

            byte[] array = _webaccess.GetFile(NewVersionUrl);
            Zipper.UnZipAll(new MemoryStream(array), temporyDirectory);

            if (!Directory.Exists(temporyDirectory))
            {
                throw new DirectoryNotFoundException("Can't upgrade, unzipped directory not found");
            }

            Assembly entryAssembly = Assembly.GetEntryAssembly();

            //From http://www.codeproject.com/Articles/31454/How-To-Make-Your-Application-Delete-Itself-Immedia
            ProcessStartInfo info = new ProcessStartInfo();
            info.Arguments = string.Format("/C choice /C Y /N /D Y /T 5 & copy /Y \"{0}\" \"{1}\"", Path.Combine(temporyDirectory, "*.*"), Path.GetDirectoryName(entryAssembly.Location));
            info.WindowStyle = ProcessWindowStyle.Hidden;
            info.CreateNoWindow = true;
            info.FileName = "cmd.exe";
            Process.Start(info);
        }
    }
}
