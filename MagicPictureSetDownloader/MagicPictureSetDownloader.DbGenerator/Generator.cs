namespace MagicPictureSetDownloader.DbGenerator
{
    using System;
    using System.Data;
    using System.Data.SqlServerCe;
    using System.IO;

    using Common.Zip;

    internal class Generator
    {
        private readonly string _connectionString;
        private readonly DbType _data;

        internal Generator(string connectionString, DbType data)
        {
            _connectionString = connectionString;
            _data = data;
            SqlCeEngine engine = new SqlCeEngine(connectionString);
            engine.CreateDatabase();
        }
        internal void Generate()
        {
            StreamReader sr = new StreamReader(Zipper.UnZipOneFile(GetStream()));
            string sqlcommand = sr.ReadToEnd();
            string[] commands = sqlcommand.Split(new[] { "GO" }, StringSplitOptions.RemoveEmptyEntries);

            using (SqlCeConnection cnx = new SqlCeConnection(_connectionString))
            {
                cnx.Open();
                using (SqlCeCommand cmd = cnx.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    foreach (string command in commands)
                    {
                        string trimcommand = command.TrimEnd(new[] { '\r', '\n' });
                        if (!string.IsNullOrWhiteSpace(trimcommand))
                        {
                            cmd.CommandText = trimcommand;
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
        }
        private byte[] GetStream()
        {
            switch (_data)
            {
                case DbType.Data:
                    return Properties.Resource.MagicData;
                case DbType.Picture:
                    return Properties.Resource.MagicPicture;
            }

            return null;
        }
    }
}
