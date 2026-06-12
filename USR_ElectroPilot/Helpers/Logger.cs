using System;
using System.IO;

namespace USR_ElectroPilot.Helpers
{
    public static class Logger
    {
        private static readonly string _logDir = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "Logs");

        private static string LogFilePath
        {
            get { return Path.Combine(_logDir, "log_" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt"); }
        }

        public static void Info(string message)
        {
            Write("INFO", message, null);
        }

        public static void Error(string message, Exception exception)
        {
            Write("ERROR", message, exception);
        }

        private static void Write(string level, string message, Exception exception)
        {
            Directory.CreateDirectory(_logDir);

            var line = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " [" + level + "] " + message;
            if (exception != null)
            {
                line += Environment.NewLine + exception;
            }

            File.AppendAllText(LogFilePath, line + Environment.NewLine);
        }
    }
}
