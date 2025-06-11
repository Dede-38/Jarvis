using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Bicyclette
{
    public static class OpenAIService
    {
        public static string ApiKey = "sk-..."; // remplace avec ta clé

        private static readonly HttpClient client = new HttpClient();

        public static async Task<string> EnvoyerMessageAsync(string message, string model = "gpt-3.5-turbo")
        {
            var url = "https://api.openai.com/v1/chat/completions";

            var payload = new
            {
                model = model,
                messages = new[]
                {
                    new { role = "user", content = message }
                }
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ApiKey);

            var response = await client.PostAsync(url, content);
            var responseString = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(responseString);
            return doc.RootElement
                      .GetProperty("choices")[0]
                      .GetProperty("message")
                      .GetProperty("content")
                      .GetString();
        }
    }
}
