using Newtonsoft.Json;

using System.Net.Http;

namespace ChromeDriverDownload
{
    public class DriverDownloadLastStableVersion
    {
        /// <summary>
        /// Se obtienen el EndPoint para descargar el archivo 'chromedriver_win32.zip', basado en:
        /// https://googlechromelabs.github.io/chrome-for-testing/#stable
        /// </summary>
        /// <param name="fullVersionChromedriver">Versión completa de Google Chrome</param>
        /// <returns>EndPoint para descargar el archivo</returns>
        private string EndPonit_Zip(string fullVersionChromedriver) => $"https://storage.googleapis.com/chrome-for-testing-public/{fullVersionChromedriver}/win32/chromedriver-win32.zip";

        /// <summary>
        /// Se obtienen el EndPoint para descargar el archivo 'chromedriver_win32.zip' de la última versión estable, basado en:
        /// https://googlechromelabs.github.io/chrome-for-testing/#stable
        /// </summary>
        /// <returns>EndPoint para descargar el archivo</returns>
        private string EndPonit_Zip() => $"https://storage.googleapis.com/chrome-for-testing-public/{GetLastKnownGoodVersion()}/win32/chromedriver-win32.zip";

        /// <summary>
        /// Obtener HttpContent de para descargar el archivo 'chromedriver_win32.zip'
        /// </summary>
        public HttpContent GetHttpContentZip(string fullVersionChromedriver) => new RestApi().GetAsyncAndReturnContent(EndPonit_Zip(fullVersionChromedriver)).GetAwaiter().GetResult();
        public HttpContent GetHttpContentZip() => new RestApi().GetAsyncAndReturnContent(EndPonit_Zip()).GetAwaiter().GetResult();

        /// <summary>
        /// Obtener la ultima versión estable de 'chromedriver_win32.zip'
        /// </summary>
        /// <returns></returns>
        public string GetLastKnownGoodVersion()
        {
            var jsonString = new RestApi().GetAsyncAndReturnStringContent("https://googlechromelabs.github.io/chrome-for-testing/last-known-good-versions-with-downloads.json").Result;
            dynamic jsonDynamic = JsonConvert.DeserializeObject<dynamic>(jsonString);

            return jsonDynamic.channels.Stable.version.Value;
        }
    }
}