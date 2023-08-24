using System;
using System.Diagnostics;
using System.IO;

namespace ChromeDriverDownload
{
    public class ChromeVersion
    {
        /// <summary>
        /// Retorna 'ProductVersion' asociado a 'Google\Chrome\Application\chrome.exe'
        /// </summary>
        /// <returns>ProductVersion de Chrome</returns>
        public string GetChromeFullVersion()
        {
            // Se obtine la informacion version del archivo.
            FileVersionInfo myFileVersionInfo = FileVersionInfo.GetVersionInfo(GetChromePath());

            return myFileVersionInfo.ProductVersion;
        }

        /// <summary>
        /// Obtiene la ruta de acceso de 'Google\Chrome\Application\chrome.exe'
        /// </summary>
        /// <returns>Full path of 'chrome.exe'</returns>
        private string GetChromePath()
        {
            //Path de "Program Files" (Con o sin x86)
            var pathProgramFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            pathProgramFiles = pathProgramFiles.Replace(" (x86)", "");
            var pathProgramFilesX86 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);

            //Path del ejecutable de Chrome.
            var pathChrome = @"Google\Chrome\Application\chrome.exe";

            //Obtengo la URI completa del ejecutable de Chrome
            string programUriX86 = Path.Combine(pathProgramFilesX86, pathChrome);
            if (File.Exists(programUriX86)) return programUriX86;

            string programUri = Path.Combine(pathProgramFiles, pathChrome);
            return File.Exists(programUri) ? programUri : null;
        }
    }
}
