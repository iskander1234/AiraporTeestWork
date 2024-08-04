using System.Text.Json;
using AirportDistanceService.Models;
using AirportDistanceService.Service.IService;

namespace AirportDistanceService.Service
{
    public class AirportService : IAirportService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<AirportService> _logger; 

        public AirportService(HttpClient httpClient, ILogger<AirportService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<AirportData> GetAirportDataAsync(string iataCode)
        {
            try
            {
                var url = $"https://places-dev.cteleport.com/airports/{iataCode}";
                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Failed to fetch data for {IataCode}. Status code: {StatusCode}", iataCode,
                        response.StatusCode);
                    return null;
                }

                var responseData = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<AirportData>(responseData,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HttpRequestException while fetching data for {IataCode}", iataCode);
                return null;
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "JsonException while deserializing data for {IataCode}", iataCode);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while fetching data for {IataCode}", iataCode);
                return null;
            }
        }
    }
}