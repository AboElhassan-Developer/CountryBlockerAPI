using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace CountryBlockerAPI.Services
{
    public class GeoLocationService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public GeoLocationService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["GeoLocationSettings:ApiKey"] ?? throw new ArgumentNullException("API Key not found in appsettings.json");
        }

        public async Task<string?> GetCountryByIP(string ipAddress)
        {
            try
            {
                string url = $"https://api.ipgeolocation.io/ipgeo?apiKey={_apiKey}&ip={ipAddress}";
                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"[ERROR] API request failed with status code {response.StatusCode}");
                    return null;
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"[DEBUG] API Response: {responseContent}");

                using var jsonDoc = JsonDocument.Parse(responseContent);

                if (jsonDoc.RootElement.TryGetProperty("country_code2", out var countryCodeElement))
                {
                    string countryCode = countryCodeElement.GetString() ?? "";
                    Console.WriteLine($"[DEBUG] Extracted Country Code: {countryCode}");
                    return countryCode;
                }
                else
                {
                    Console.WriteLine("[ERROR] 'country_code2' not found in API response.");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Exception: {ex.Message}");
                return null;
            }
        }
    }
}
