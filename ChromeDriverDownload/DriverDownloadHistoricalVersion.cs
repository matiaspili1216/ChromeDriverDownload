using System.Net.Http;

namespace ChromeDriverDownload
{
    public class DriverDownloadHistoricalVersion
    {
        private readonly string VersionChromedriver;

        private string GetEndPonit(string version) => $"https://chromedriver.storage.googleapis.com/LATEST_RELEASE_{version}";
        private string GetEndPonit_Zip(string releaseChromedriver) => $"https://chromedriver.storage.googleapis.com/{releaseChromedriver}/chromedriver_win32.zip";

        public DriverDownloadHistoricalVersion(string versionChromedriver) => VersionChromedriver = versionChromedriver;


        /// <summary>
        /// Obterner el Release de Chromedriver.exe
        /// </summary>
        /// <returns>Release de de Chromedriver</returns>
        private string GetChromedriverRelease() => new RestApi().GetAsyncAndReturnStringContent(GetEndPonit(VersionChromedriver)).GetAwaiter().GetResult();

        /// <summary>
        /// Obtener HttpContent de para descargar el archivo 'chromedriver_win32.zip'
        /// </summary>
        public HttpContent GetHttpContentZip() => new RestApi().GetAsyncAndReturnContent(GetEndPonit_Zip(GetChromedriverRelease())).GetAwaiter().GetResult();
    }
}