using System;
using System.Data.SQLite;
using System.IO;

namespace USR_ElectroPilot.Data
{
    public static class SqliteConnectionFactory
    {
        public const string DatabaseFileName = "usr_electropilot.db";

        public static string DatabaseDirectory
        {
            get { return Path.Combine(GetProjectRoot(), "Database"); }
        }

        public static string DatabasePath
        {
            get { return Path.Combine(DatabaseDirectory, DatabaseFileName); }
        }

        public static SQLiteConnection CreateConnection()
        {
            Directory.CreateDirectory(DatabaseDirectory);

            return new SQLiteConnection(
                "Data Source=" + DatabasePath + ";Version=3;Foreign Keys=True;Journal Mode=WAL;");
        }

        private static string GetProjectRoot()
        {
            var directory = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);

            while (directory != null)
            {
                if (File.Exists(Path.Combine(directory.FullName, "USR_ElectroPilot.csproj")))
                {
                    return directory.FullName;
                }

                directory = directory.Parent;
            }

            return AppDomain.CurrentDomain.BaseDirectory;
        }
    }
}
