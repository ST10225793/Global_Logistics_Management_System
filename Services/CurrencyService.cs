using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Global_Logistics_Management_System.Services
{
    public class CurrencyService
    {
        private readonly HttpClient _httpClient;
        private const decimal FALLBACK_EXCHANGE_RATE = 18.50m;

        public CurrencyService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// Fetches the live evaluation multiplier via an external open API channel.
        /// Incorporates try/catch safe failguards to prevent system crashes if offline.
        /// </summary>
        public async Task<decimal> GetZarRateAsync(string baseCurrency = "USD")
        {
            try
            {
                // Utilizing standard free open API endpoint structures 
                string connectionUrl = $"https://open.er-api.com/v6/latest/{baseCurrency}";

                // Fetch mapping parameters asynchronously 
                var response = await _httpClient.GetAsync(connectionUrl);

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();

                    // Enforce case-insensitive deserialization behaviors for properties safely
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var payloadData = JsonSerializer.Deserialize<ExchangeRateApiResponse>(jsonString, options);

                    if (payloadData != null && payloadData.Rates != null && payloadData.Rates.TryGetValue("ZAR", out decimal zarMultiplier))
                    {
                        // Return precise mathematical conversion value metrics 
                        return zarMultiplier;
                    }
                }

                System.Diagnostics.Debug.WriteLine("API ALERT: Connection succeeded but payload parameters were unreadable. Deploying fallback rate.");
                return FALLBACK_EXCHANGE_RATE;
            }
            catch (HttpRequestException netEx)
            {
                // Logs network-specific drop metrics safely to output streams without breaking execution loops
                System.Diagnostics.Debug.WriteLine($"API CONNECTION ERROR (HttpClient): {netEx.Message}. Using connection fallbacks: R {FALLBACK_EXCHANGE_RATE}");
                return FALLBACK_EXCHANGE_RATE;
            }
            catch (Exception generalEx)
            {
                // Graceful fallback shield to ensure operations continue uninterrupted during generic processing errors
                System.Diagnostics.Debug.WriteLine($"API GENERAL PROCESSING EXCEPTION: {generalEx.Message}. Enforcing backup calculation bounds.");
                return FALLBACK_EXCHANGE_RATE;
            }
        }
    }

    // =========================================================================
    // DEPENDENT API DTO LAYER (Matches open.er-api.com root schemas)
    // =========================================================================

    public class ExchangeRateApiResponse
    {
        [JsonPropertyName("result")]
        public string Result { get; set; } = string.Empty;

        [JsonPropertyName("base_code")]
        public string BaseCode { get; set; } = string.Empty;

        [JsonPropertyName("rates")]
        public Dictionary<string, decimal> Rates { get; set; } = new Dictionary<string, decimal>();
    }
}