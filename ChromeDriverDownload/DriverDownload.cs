using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;

namespace ChromeDriverDownload
{
    public class DriverDownload
    {
        private string InitialPath => @"C:\Automation\Webdrivers";
        private string DefaultVersion => "114";
        private string NameFileZip => "chromedriver_win32.zip";
        private string NameFile => "chromedriver.exe";

        /// <summary>
        /// Descargar, si no existe, 'chromedriver.exe' y retornar la carpeta en donde se encuentra.
        /// Determina como descargar, a partir de la versión
        /// </summary>
        /// <returns>Ruta de la carpeta donde se encuentra el 'chromedriver.exe' necesario para la versión de 'chrome.exe' instalada</returns>
        public string DownloadDriverAndGetFolderPath()
        {
            var chromeFullVersion = new ChromeVersion().GetChromeFullVersion();

            // Se obtine la informacion del número de version o el número de version por defecto.
            string chromeVersion = chromeFullVersion?.Split('.').FirstOrDefault() ?? DefaultVersion;

            bool isLastStable = int.Parse(chromeVersion) > 114;

            // Se crean los Path correspondientes de la carpeta final y del archivo final.
            var pathfolderDownload = Path.Combine(InitialPath, chromeVersion);
            var pathfolderFull = isLastStable ? Path.Combine(InitialPath, chromeVersion, "chromedriver-win32") : Path.Combine(InitialPath, chromeVersion);

            var pathFile = Path.Combine(pathfolderFull, NameFile);

            //Si no existe la carpeta, no existe el 'chromedriver.exe' expesifico para la versión de 'chrome'. Por lo tanto, se crea y se descarga.

            if (!File.Exists(pathFile))
            {
                Directory.CreateDirectory(pathfolderDownload);
                DownloadDriver(pathfolderDownload, chromeFullVersion, isLastStable);
            }

            return pathfolderFull;
        }
        
        /// <summary>
        /// Descargar 'chromedriver.exe'
        /// </summary>
        /// <param name="pathfolderToDownload">Carpera destino</param>
        /// <param name="productFullVersion">Versión de Google Chrome</param>
        private void DownloadDriver(string pathfolderToDownload, string productFullVersion, bool isLastStable = true)
        {
            HttpContent httpContentZip = isLastStable ? 
                    new DriverDownloadLastStableVersion().GetHttpContentZip() :
                    new DriverDownloadHistoricalVersion(productFullVersion).GetHttpContentZip();

            string zipFilePath = Path.Combine(pathfolderToDownload, NameFileZip);

            DownloadZip(httpContentZip, zipFilePath);

            Unzip(pathfolderToDownload, zipFilePath);

            File.Delete(zipFilePath);
        }

        /// <summary>
        /// Descargar el archivo 'chromedriver_win32.zip'
        /// </summary>
        /// <param name="releaseChromedriver">Release de Chromedriver a descargar</param>
        /// <param name="fileZipPath">Carpeta destino</param>
        private void DownloadZip(HttpContent httpContentZip, string zipFilePath)
        {
            //Si existe un archivo descargado, se elimina.
            if (File.Exists(zipFilePath)) { File.Delete(zipFilePath); }

            FileStream stream = new FileStream(zipFilePath, FileMode.CreateNew);
            httpContentZip.CopyToAsync(stream).GetAwaiter().GetResult();
            stream.Dispose();
        }

        /// <summary>
        /// Extraer los archivos de un .ZIP
        /// </summary>
        /// <param name="destinationDirectoryName">Carpera destino de la extracción</param>
        /// <param name="sourceArchiveFileName">Archivo a extraer</param>
        private void Unzip(string destinationDirectoryName, string sourceArchiveFileName)
        {
            string filePath = Path.Combine(destinationDirectoryName, NameFileZip);
            ZipFile.ExtractToDirectory(sourceArchiveFileName, destinationDirectoryName);
            if (File.Exists(filePath)) { File.Delete(filePath); }

        }

    }
}