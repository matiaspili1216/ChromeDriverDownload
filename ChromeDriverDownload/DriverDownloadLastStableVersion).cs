using Newtonsoft.Json;

using System.Net.Http;

namespace ChromeDriverDownload
{
    public class DriverDownloadLastStableVersion
    {
        private string EndPonit_Zip(string fullVersionChromedriver) => $"https://edgedl.me.gvt1.com/edgedl/chrome/chrome-for-testing/{fullVersionChromedriver}/win32/chromedriver-win32.zip";
        private string EndPonit_Zip() => $"https://edgedl.me.gvt1.com/edgedl/chrome/chrome-for-testing/{GetLastKnownGoodVersion()}/win32/chromedriver-win32.zip";

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