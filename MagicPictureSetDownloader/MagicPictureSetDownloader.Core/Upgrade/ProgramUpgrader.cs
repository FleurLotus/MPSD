namespace MagicPictureSetDownloader.Core.Upgrade
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Reflection;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;

    using Common.Web;
    using Common.Zip;

    using MagicPictureSetDownloader.Interface;

    using Microsoft.Extensions.Logging;

    public enum UpgradeStatus
    {
        NotChecked,
        UpToDate,
        NeedToBeUpdated,
        CantCheck,
    }

    public class ProgramUpgrader
    {
        private static readonly ILogger Logger = LogManager.Factory.CreateLogger<ProgramUpgrader>();
        private readonly WebAccess _webaccess = new WebAccess();
        private Uri _newVersionUrl;

        public ProgramUpgrader()
        {
            Status = UpgradeStatus.NotChecked;
        }

        public UpgradeStatus Status { get; private set; }

        internal static async Task<GitHubRelease> GetLatestPublish(WebAccess webAccess, string owner, string repo, CancellationToken ct)
        {
            ArgumentNullException.ThrowIfNull(webAccess);
            if (string.IsNullOrWhiteSpace(owner))
            {
                throw new ArgumentException("owner is required", nameof(owner));
            }
            if (string.IsNullOrWhiteSpace(repo))
            {
                throw new ArgumentException("repo is required", nameof(repo));
            }

            try
            {
                string releaseUrl = $"https://api.github.com/repos/{owner}/{repo}/releases/latest";
                string json = await webAccess.GetHtmlAsync(releaseUrl, true, ct).ConfigureAwait(false);
                if (!string.IsNullOrWhiteSpace(json))
                {
                    return JsonSerializer.Deserialize<GitHubRelease>(json);
                }
            }
            catch (WebException ex)
            {
                Logger.LogError(ex, "Can't get latest release from GitHub");
                // network error or authentication required - swallow and try artifact fallback
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Can't get latest release from GitHub");
                // non-fatal parse error - fallback to artifacts
            }

            return null;
        }

        public async Task<bool> HasNewVersionAvailable(CancellationToken ct)
        {
            try
            {
                GitHubRelease release = await GetLatestPublish(_webaccess, "FleurLotus", "MPSD", ct).ConfigureAwait(false) ?? throw new ProgramUpgraderException("Can't get info from latest version");
                GitHubAsset asset = release.Assets.FirstOrDefault(a => a.Name.Equals("MPSD.zip", StringComparison.OrdinalIgnoreCase)) ?? throw new ProgramUpgraderException("Can't get asset from latest version");
                _newVersionUrl = asset.BrowserDownloadUrl;
                string releaseVersion = release.TagName.Replace("Version_", string.Empty);
                Version newVersionNumberVersion = new Version(releaseVersion);

                Assembly entryAssembly = Assembly.GetEntryAssembly() ?? throw new ProgramUpgraderException("Can't get entry assembly");
                Version currentVersion = entryAssembly.GetName().Version;

                bool hasNewVersion = currentVersion < newVersionNumberVersion;
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

        public async Task Upgrade(bool alreadyChecked, CancellationToken ct)
        {
            if (_newVersionUrl == null)
            {
                throw new ProgramUpgraderException("Can't get info from new version file");
            }

            if (!alreadyChecked && !await HasNewVersionAvailable(ct).ConfigureAwait(false))
            {
                throw new ProgramUpgraderException("No call of upgrade if HasNewVersionAvailable is false");
            }

            string temporyDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString().ToUpperInvariant());

            byte[] array = await _webaccess.GetFileAsync(_newVersionUrl.ToString(), ct).ConfigureAwait(false);
            Zipper.UnZipAll(new MemoryStream(array), temporyDirectory);

            if (!Directory.Exists(temporyDirectory))
            {
                throw new DirectoryNotFoundException("Can't upgrade, unzipped directory not found");
            }

            Assembly entryAssembly = Assembly.GetEntryAssembly() ?? throw new ProgramUpgraderException("Can't get entry assembly");

            //From http://www.codeproject.com/Articles/31454/How-To-Make-Your-Application-Delete-Itself-Immedia
            ProcessStartInfo info = new ProcessStartInfo
            {
                Arguments = string.Format("/C choice /C Y /N /D Y /T 5 & copy /Y \"{0}\" \"{1}\"", Path.Combine(temporyDirectory, "*.*"), Path.GetDirectoryName(entryAssembly.Location)),
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true,
                FileName = "cmd.exe"
            };
            Process.Start(info);
        }
    }
}