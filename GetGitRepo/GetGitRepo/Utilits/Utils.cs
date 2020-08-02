using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GetGitRepo.Utilits
{
    public static class Utils
    {
        /// <summary>
        /// Get a file extension
        /// </summary>
        /// <param name="fileName">(string) File Name</param>
        /// <returns>(string) File Extesion</returns>
        public static string getFileExtension(string fileName)
        {
            var texto = fileName.Split(".");
            return texto.Last();
        }
    }
}
