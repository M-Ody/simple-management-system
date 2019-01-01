using SysGuiApi.Control;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SysGuiApi.Services
{
    public static class LogService
    {
        static string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\SysGui\\";
        static string filePath, fileName;
        static DateTime currentDate;

        public static void WriteLog(string userSignature, string line)
        {
            if (currentDate < DateTime.Today)
                FormatPath();

            Directory.CreateDirectory(filePath);

            using (StreamWriter outputFile = new StreamWriter(Path.Combine(filePath, fileName), true))
            {
                outputFile.WriteLine(SignatureManager.Instance.GetUsername(userSignature) + " - " + line);
            }
        }

        private static void FormatPath()
        {
            var today = DateTime.Today;
            filePath = path + today.Year + "\\" + today.Month + "\\";
            fileName = today.Day + "_LOG.txt";
        }
    }
}
