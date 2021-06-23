using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;

namespace ChromeDriverDownload
{
    public class DriverDownload
    {
        private string PathToDownload => @"C:\Webdrivers";
        private string DefaultVersion => "88";
        private string NameFileZip => "chromedriver_win32.zip";
        private string NameFile => "chromedriver.exe";

        private string GetEndPonit_ReleaseChromedriver(string version) => $"https://chromedriver.storage.googleapis.com/LATEST_RELEASE_{version}";
        private string GetEndPonit_Zip(string releaseChromedriver) => $"https://chromedriver.storage.googleapis.com/{releaseChromedriver}/chromedriver_win32.zip";

        /// <summary>
        /// Descargar, si no existe, 'chromedriver.exe' y retornar la carpeta en donde se encuentra.
        /// </summary>
        /// <returns>Ruta de la carpeta donde se encuentra el 'chromedriver.exe' necesario para la versión de 'chrome.exe' instalada</returns>
        public string DownloadDriverAndGetFolderPath()
        {
            string pathOfChromeExe = GetPathOfChromeExe();
            if(string.IsNullOrEmpty(pathOfChromeExe)) { throw new Exception("Google Chrome no se encuentra instalado"); }

            // Se obtine la informacion version del archivo, en su formato completo.
            string chromeFullVersion = FileVersionInfo.GetVersionInfo(pathOfChromeExe)?.ProductVersion;

            // Se obtine la informacion del número de version o el número de version por defecto.
            string chromeVersion = chromeFullVersion?.Split('.').FirstOrDefault() ?? DefaultVersion;

            // Se crean los Path correspondientes de la carpeta final y del archivo final.
            var pathFolder = Path.Combine(PathToDownload, chromeVersion);
            var pathFile = Path.Combine(pathFolder, NameFile);

            //Si no existe la carpeta, no existe el 'chromedriver.exe' expesifico para la versión de 'chrome'. Por lo tanto, se crea y se descarga.
            if (!File.Exists(pathFile))
            {
                Directory.CreateDirectory(pathFolder);
                DownloadChromeDriver(pathFolder, chromeFullVersion);
            }

            return pathFolder;
        }

        /// <summary>
        /// Obtener la ruta completa del ejecutable de Chrome
        /// </summary>
        /// <returns>Uri path completa del archivo chrome.exe</returns>
        private string GetPathOfChromeExe()
        {
            //Path de "Program Files" (Con o sin x86)
            var pathProgramFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            var pathProgramFilesX86 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);

            //Path del ejecutable de Chrome.
            var pathChrome = @"Google\Chrome\Application\chrome.exe";

            //Se obtiene la URI completa del ejecutable de Chrome.            
            string programUriX86 = Path.Combine(pathProgramFilesX86, pathChrome);
            if (File.Exists(programUriX86))
            {
                return programUriX86; //Si existe dentro de ProgramFilesX86, se retorna. Si no, se busca en ProgramFiles
            }
            else
            {
                string programUri = Path.Combine(pathProgramFiles, pathChrome);
                return File.Exists(programUri) ? programUri : null; //Si existe dentro de ProgramFiles, se retorna. Si no, se retona null
            }

        }

        /// <summary>
        /// Descargar 'chromedriver.exe'
        /// </summary>
        /// <param name="pathfolderToDownload">Carpera destino</param>
        /// <param name="productFullVersion">Versión de Google Chrome</param>
        private void DownloadChromeDriver(string pathfolderToDownload, string productFullVersion)
        {
            //Se descarga el Zip que contiene el archivo
            string zipFilePath = DownloadZipGetFilePath(pathfolderToDownload, productFullVersion);

            //Se extrae el Zip
            Unzip(pathfolderToDownload, zipFilePath);

            //Se elimina el Zip, dejando solo 'chromedriver.exe'
            File.Delete(zipFilePath);
        }

        /// <summary>
        /// Descargar el archivo 'chromedriver_win32.zip', y retornar la ruta de descarga
        /// </summary>
        /// <param name="pathfolderToDownload">Carpera destino</param>
        /// <param name="productFullVersion">Versión de Google Chrome</param>
        /// <returns>Ruta de descarga</returns>
        private string DownloadZipGetFilePath(string pathfolderToDownload, string productFullVersion)
        {
            //Se calcula la versión de Chrome que se utiliza para la busqueda (sin el último número)
            string versionLast = productFullVersion.Split('.').Last();
            string versionToSearch = productFullVersion.Replace($".{versionLast}", "");

            string releaseChromedriver = GetChromedriverRelease(versionToSearch);

            string filePath = Path.Combine(pathfolderToDownload, NameFileZip);

            DownloadZip(releaseChromedriver, filePath);

            return filePath;
        }

        /// <summary>
        /// Extraer los archivos de un .ZIP
        /// </summary>
        /// <param name="destinationDirectoryName">Carpera destino de la extracción</param>
        /// <param name="sourceArchiveFileName">Archivo a extraer</param>
        private void Unzip(string destinationDirectoryName, string sourceArchiveFileName)
        {
            string filePath = Path.Combine(destinationDirectoryName, NameFile);
            if (File.Exists(filePath)) { File.Delete(filePath); }
            ZipFile.ExtractToDirectory(sourceArchiveFileName, destinationDirectoryName);
        }

        /// <summary>
        /// Descargar el archivo 'chromedriver_win32.zip'
        /// </summary>
        /// <param name="releaseChromedriver">Release de Chromedriver a descargar</param>
        /// <param name="fileZipPath">Carpeta destino</param>
        private void DownloadZip(string releaseChromedriver, string fileZipPath)
        {
            //Si existe un archivo descargado, se elimina.
            if (File.Exists(fileZipPath)) { File.Delete(fileZipPath); }

            // Se descarga el contenido del archvo, y se compia en el archivo destino
            HttpContent stringFile = new RestApi().GetAsyncAndReturnContent(GetEndPonit_Zip(releaseChromedriver)).GetAwaiter().GetResult();

            FileStream stream = new FileStream(fileZipPath, FileMode.CreateNew);
            stringFile.CopyToAsync(stream).GetAwaiter().GetResult();
            stream.Dispose();
        }

        /// <summary>
        /// Obterner el Release de Chromedriver.exe
        /// </summary>
        /// <param name="versionOfChrome"></param>
        /// <returns>Release de de Chromedriver</returns>
        private string GetChromedriverRelease(string versionOfChrome) => new RestApi().GetAsyncAndReturnStringContent(GetEndPonit_ReleaseChromedriver(versionOfChrome)).GetAwaiter().GetResult();
    }
}