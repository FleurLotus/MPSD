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
        private const int LevelSize = 2;

        private const string RootFolder = "MagicPicture";
        private const string CardFolder = "Card";
        private const string TreeFolder = "Tree";

        private readonly string _cardPath;
        private readonly string _treePath;

        private readonly IDictionary<string, ITreePicture> _treePictures = new Dictionary<string, ITreePicture>(StringComparer.InvariantCultureIgnoreCase);
        private readonly IDictionary<string, IPicture> _pictures = new Dictionary<string, IPicture>();

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
                TreePicture treePicture = new TreePicture { Name = Path.GetFileNameWithoutExtension(file), Image = File.ReadAllBytes(file), FilePath = file };
                if (!_treePictures.ContainsKey(treePicture.Name))
                {
                    _treePictures.Add(treePicture.Name, treePicture);
                }
            }
        }
        public ITreePicture GetTreePicture(string key)
        {
            if (key == "@∞")
            {
                key = "@infinity";
            }
            if (key == "@½")
            {
                key = "@0.5";
            }

            return _treePictures.GetOrDefault(ToWindowsFileName(key));
        }
        public IPicture GetPicture(string idScryFall, bool doNotCache = false)
        {
            if (!_pictures.TryGetValue(idScryFall, out IPicture picture))
            {
                string path = GeneratePath(idScryFall);
                path = Path.GetDirectoryName(Path.Combine(_cardPath, path));

                if (Directory.Exists(path))
                {
                    foreach (string file in Directory.GetFiles(path, $"{idScryFall}.*", SearchOption.AllDirectories))
                    {
                        picture = new Picture { IdScryFall = idScryFall, Image = File.ReadAllBytes(file) };
                        if (!doNotCache)
                        {
                            _pictures.Add(picture.IdScryFall, picture);
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

        public void InsertNewTreePicture(string name, byte[] data, bool isSvg)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return;
            }

            if (data == null || data.Length == 0)
            {
                return;
            }

            string path = GeneratePathFromName(name);
            string ext = isSvg ? ".svg" : GetExtension(data);
            string filePath = Path.Combine(_treePath, path + ext);

            if (Directory.GetFiles(Path.GetDirectoryName(filePath), Path.GetFileNameWithoutExtension(filePath) + ".*").Length> 0)
            {
                return;
            }

            Save(filePath, data);

            TreePicture treePicture = new TreePicture { Name = Path.GetFileNameWithoutExtension(filePath), Image = data, FilePath = filePath };
            _treePictures.Add(treePicture.Name, treePicture);
        }
        public void InsertNewPicture(string idScryFall, byte[] data)
        {
            if (data == null || data.Length == 0)
            {
                return;
            }

            string path = GeneratePath(idScryFall);
            string ext = GetExtension(data);
            string filePath = Path.Combine(_cardPath, path + ext);

            if (Directory.Exists(Path.GetDirectoryName(filePath)) && Directory.GetFiles(Path.GetDirectoryName(filePath), Path.GetFileNameWithoutExtension(filePath) + ".*").Length > 0)
            {
                return;
            }

            Save(filePath, data);
        }
        public IPicture GetPicture(string idScryFall)
        {
            string path = GeneratePath(idScryFall);

            if (!File.Exists(path))
            {
                return null;
            }

            return new Picture { IdScryFall = idScryFall, Image = File.ReadAllBytes(path) };
        }
        private string GetExtension(byte[] bytes)
        {
            return bytes.ToImage().GetFileExtension();
        }
        private string GeneratePath(string idScryFall)
        {
            string path = string.Empty;

            for (int j = 0; j < Level; j++)
            {
                path = Path.Combine(path, idScryFall.Substring(LevelSize * j, LevelSize));
            }

            return Path.Combine(path, idScryFall.ToString());
        }
        private string GeneratePathFromName(string name)
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
