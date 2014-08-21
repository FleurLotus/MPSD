namespace MagicPictureSetDownloader.DbGenerator
{
    using System;
    using System.Data;
    using System.Data.SqlServerCe;
    using System.IO;

    using Common.Zip;

    internal class GeneratorBase
    {
        private readonly string _connectionString;

        internal GeneratorBase(string connectionString)
        {
            _connectionString = connectionString;
            SqlCeEngine engine = new SqlCeEngine(connectionString);
            engine.CreateDatabase();

        }
        internal void Generate(byte[] stream)
        {
            StreamReader sr = new StreamReader(Zipper.UnZipOneFile(stream));
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
    }
}
