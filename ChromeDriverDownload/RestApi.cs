using System.Net.Http;
using System.Threading.Tasks;

namespace ChromeDriverDownload
{
    public class RestApi
    {
        /// <summary>
        /// Realizar una petición GET, y obtener el continido de la respuesta
        /// </summary>
        /// <param name="requestUri">Request Uri de la llamada</param>
        /// <returns>Continido de la respuesta</returns>
        public async Task<HttpContent> GetAsyncAndReturnContent(string requestUri)
        {
            using var client = new HttpClient();

            return (await client.GetAsync(requestUri))?.Content;
        }

        /// <summary>
        /// Realizar una petición GET, y obtener el continido de la respuesta como String
        /// </summary>
        /// <param name="requestUri">Request Uri de la llamada</param>
        /// <returns>Continido de la respuesta como String</returns>
        public async Task<string> GetAsyncAndReturnStringContent(string requestUri)
        {
            string resultContent = (await GetAsyncAndReturnContent(requestUri))?.ReadAsStringAsync()?.Result;

            return string.IsNullOrEmpty(resultContent)
                ? throw new HttpRequestException("La ejecución del GET no obtuvo resultados.")
                : resultContent;
        }
    }
}