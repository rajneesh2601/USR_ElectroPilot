using System;
using System.Data.SQLite;
using System.IO;

namespace USR_ElectroPilot.Data
{
    public static class SqliteConnectionFactory
    {
        private static readonly string _dbPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
            "USRElectroPilot",
            "usr_electropilot.db");

        public static string DatabaseDirectory
        {
            get { return Path.GetDirectoryName(_dbPath); }
        }

        public static string DatabasePath
        {
            get { return _dbPath; }
        }

        public static SQLiteConnection CreateConnection()
        {
            Directory.CreateDirectory(DatabaseDirectory);

            return new SQLiteConnection(
                "Data Source=" + DatabasePath + ";Version=3;Foreign Keys=True;Journal Mode=WAL;");
        }

    }
}
