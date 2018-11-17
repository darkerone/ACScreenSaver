using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ACScreenSaver
{
    public static class Logger
    {
        static string _logFilePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\ACScreenSaver\ACScreenSaver_Logs.log";

        public static void LogDebug(string message)
        {
#if DEBUG
            Log("DEBUG", message);
#endif
        }

        public static void LogError(string message)
        {
            Log("ERROR", message);
        }

        public static void Log(string level, string message)
        {
            try
            {
                string log = GetCurrentTimeString() + "__" + level + "__" + message;
                System.IO.File.AppendAllLines(_logFilePath, new string[] { log });
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        /// <summary>
        /// Renvoie l'heure et la date actuelle au format string
        /// </summary>
        /// <returns></returns>
        private static string GetCurrentTimeString()
        {
            return DateTime.Now.ToString();
        }
    }
}
