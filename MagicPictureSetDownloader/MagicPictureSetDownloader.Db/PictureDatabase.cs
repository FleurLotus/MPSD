namespace MagicPictureSetDownloader.Db
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;

    using Common.Drawing;
    using Common.Library.Extension;

    using MagicPictureSetDownloader.Db.DAO;
    using MagicPictureSetDownloader.DbGenerator;
    using MagicPictureSetDownloader.Interface;

    internal class PictureDatabase
    {
        private const int Level = 3;
        private const int LevelSize = 100;
        private static readonly string Format = new string('0', 1 + (int)Math.Truncate(Math.Log10(LevelSize - 1)));

        private const string RootFolder = "MagicPicture";
        private const string CardFolder = "Card";
        private const string TreeFolder = "Tree";

        private readonly string _cardPath;
        private readonly string _treePath;

        private readonly IDictionary<string, ITreePicture> _treePictures = new Dictionary<string, ITreePicture>(StringComparer.InvariantCultureIgnoreCase);
        private readonly IDictionary<int, IPicture> _pictures = new Dictionary<int, IPicture>();

        public PictureDatabase()
        {
            DatabaseGenerator.GeneratePictures();
            string rootPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), RootFolder);
            _cardPath = Path.Combine(rootPath, CardFolder);
            _treePath = Path.Combine(rootPath, TreeFolder);
        }

        public void LoadAllTreePicture()
        {
            _treePictures.Clear();

            if (!Directory.Exists(_treePath))
            {
                return;
            }

            foreach(string file in Directory.GetFiles(_treePath, "*.*", SearchOption.AllDirectories))
            {
                TreePicture treePicture = new TreePicture { Name = Path.GetFileNameWithoutExtension(file), Image = File.ReadAllBytes(file) };
                if (!_treePictures.ContainsKey(treePicture.Name))
                {
                    _treePictures.Add(treePicture.Name, treePicture);
                }
            }
        }
        public ITreePicture GetTreePicture(string key)
        {
            return _treePictures.GetOrDefault(ToWindowsFileName(key));
        }
        public IPicture GetPicture(int idGatherer, bool doNotCache = false)
        {
            IPicture picture;

            if (!_pictures.TryGetValue(idGatherer, out picture))
            {
                string path = GeneratePath(idGatherer);
                path = Path.GetDirectoryName(Path.Combine(_cardPath, path));

                if (Directory.Exists(path))
                {
                    foreach (string file in Directory.GetFiles(path, idGatherer.ToString() + ".*", SearchOption.AllDirectories))
                    {
                        picture = new Picture { IdGatherer = idGatherer, Image = File.ReadAllBytes(file) };
                        if (!doNotCache)
                        {
                            _pictures.Add(picture.IdGatherer, picture);
                        }

                        break;
                    }
                }
            }

            return picture;
        }

        public IEnumerable<int> GetAllPictureIds()
        {
            if (!Directory.Exists(_cardPath))
            {
                yield break;
            }

            foreach (string file in Directory.GetFiles(_cardPath, "*.*", SearchOption.AllDirectories))
            {
                string name = Path.GetFileNameWithoutExtension(file);

                if (int.TryParse(name, out int id))
                {
                    yield return id;
                }
            }
        }

        public void InsertNewTreePicture(string name, byte[] data)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return;
            }

            if (data == null || data.Length == 0)
            {
                return;
            }

            string path = GeneratePath(name);
            string ext = GetExtension(data);
            string filePath = Path.Combine(_treePath, path + ext);

            if (Directory.GetFiles(Path.GetDirectoryName(filePath), Path.GetFileNameWithoutExtension(filePath) + ".*").Length> 0)
            {
                return;
            }

            Save(filePath, data);

            TreePicture treePicture = new TreePicture { Name = Path.GetFileNameWithoutExtension(filePath), Image = data };
            _treePictures.Add(treePicture.Name, treePicture);
        }
        public void InsertNewPicture(int idGatherer, byte[] data)
        {
            if (data == null || data.Length == 0)
            {
                return;
            }

            string path = GeneratePath(idGatherer);
            string ext = GetExtension(data);
            string filePath = Path.Combine(_cardPath, path + ext);

            if (Directory.Exists(Path.GetDirectoryName(filePath)) && Directory.GetFiles(Path.GetDirectoryName(filePath), Path.GetFileNameWithoutExtension(filePath) + ".*").Length > 0)
            {
                return;
            }

            Save(filePath, data);
        }
        public IPicture GetPicture(int idGatherer)
        {
            string path = GeneratePath(idGatherer);

            if (!File.Exists(path))
            {
                return null;
            }

            return new Picture { IdGatherer = idGatherer, Image = File.ReadAllBytes(path) };
        }
        private string GetExtension(byte[] bytes)
        {
            return bytes.ToImage().GetFileExtension();
        }
        private string GeneratePath(int idGatherer)
        {
            string path = string.Empty;

            int id = Math.Abs(idGatherer);

            int[] levels = new int[Level];

            int coef = LevelSize;

            for (int i = 0; i < Level; i++)
            {
                int l = (id / coef) % LevelSize;
                levels[i] = l;
                coef *= LevelSize;
            }

            for (int j= Level -1; j>=0; j--)
            {
                path = Path.Combine(path, levels[j].ToString(Format));
            }

            return Path.Combine(path, idGatherer.ToString());
        }
        private string GeneratePath(string name)
        {
            return ToWindowsFileName(name.ToLowerInvariant());
        }
        private void Save(string filePath, byte[] bytes)
        {
            string directory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            using (FileStream fs = new FileStream(filePath, FileMode.CreateNew))
            {
                fs.Write(bytes, 0, bytes.Length);
            }
        }
        private string ToWindowsFileName(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return key;
            }
            if (key.IndexOfAny(Path.GetInvalidFileNameChars()) == -1)
            {
                return key;
            }

            foreach (char c in Path.GetInvalidFileNameChars())
            {
                key = key.Replace(c, '_');
            }

            return key;
        }
    }
}
