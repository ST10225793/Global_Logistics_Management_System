using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Global_Logistics_Management_System.Models;

namespace Global_Logistics_Management_System.Services
{
    public class CurrencyService
    {
        private readonly HttpClient _httpClient;

        public CurrencyService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<decimal> GetZarRateAsync(string baseCurrency = "USD")
        {
            try
            {
                // Using ExchangeRate-API's free open endpoint
                string url = $"https://open.er-api.com/v6/latest/{baseCurrency}";
                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var data = JsonSerializer.Deserialize<ExchangeRateResponse>(jsonString);

                    if (data != null && data.ConversionRates.ContainsKey("ZAR"))
                    {
                        return data.ConversionRates["ZAR"];
                    }
                }

                // Fallback hardcoded default exchange rate in case the external API goes offline
                return 18.50m;
            }
            catch
            {
                // Fallback rate to prevent the application from crashing if internet access drops out
                return 18.50m;
            }
        }
    }
}