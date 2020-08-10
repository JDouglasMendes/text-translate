using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace TextTranslate
{
    public class Translator : IDisposable
    {
        private readonly HttpClient httpClient;
        private readonly HttpRequestMessage httpRequestMessage;
        private readonly string endpoint;
        private readonly SecureString subscriptionKey;
        private bool disposedValue;
        private string languageTranslate;
        private string fromLanguage;
        private bool addFrom;

        public static Translator CreateFromKeys(string endpoint, string subscriptionKey, string language = null)
            => new Translator(endpoint, subscriptionKey, language);

        internal void AddFrom()
            => addFrom = true;

        internal void AddLanguage(string language)
        {
            if (addFrom)
            {
                fromLanguage = "&from=" + language;
                addFrom = false;
                return;
            }
            languageTranslate += "&to=" + language;
        }

        private Translator(string endpoint, string subscriptionKey, string language = null)
        {
            if (string.IsNullOrEmpty(endpoint))
                throw new ArgumentException($"{nameof(endpoint)} not found");

            if (string.IsNullOrEmpty(subscriptionKey))
                throw new ArgumentException($"{nameof(subscriptionKey)} not found");

            this.httpClient = new HttpClient();
            this.httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post
            };
            this.endpoint = endpoint;
            this.languageTranslate = language;
            this.fromLanguage = string.Empty;
            this.subscriptionKey = new SecureString();
            Array.ForEach(subscriptionKey.ToCharArray(), this.subscriptionKey.AppendChar);
        }

        public async Task<TranslationResult[]> TranslateText(string inputText)
        {
            if (string.IsNullOrEmpty(languageTranslate))
                throw new ArgumentException("Please, configure language destiny");

            object[] body = new object[] { new { Text = inputText } };
            var requestBody = JsonConvert.SerializeObject(body);

            httpRequestMessage.RequestUri = new Uri(endpoint + fromLanguage + languageTranslate);
            httpRequestMessage.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
            httpRequestMessage.Headers.Add("Ocp-Apim-Subscription-Key",
                                            new System.Net.NetworkCredential(string.Empty, subscriptionKey).Password);

            HttpResponseMessage response = await httpClient.SendAsync(httpRequestMessage).ConfigureAwait(false);
            string result = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TranslationResult[]>(result);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    httpClient.Dispose();
                    httpRequestMessage.Dispose();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}