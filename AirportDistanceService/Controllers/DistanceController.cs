using Microsoft.AspNetCore.Mvc;
using AirportDistanceService.Service.IService;
using System.Threading.Tasks;
using System;
using AirportDistanceService.Models;

namespace AirportDistanceService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DistanceController : ControllerBase
    {
        private readonly IAirportService _airportService;
        private readonly ILogger<DistanceController> _logger;

        public DistanceController(IAirportService airportService, ILogger<DistanceController> logger)
        {
            _airportService = airportService;
            _logger = logger;
        }

        [HttpGet]
        [Route("calculate")]
        public async Task<IActionResult> CalculateDistance(string airport1, string airport2)
        {
            _logger.LogInformation("Calculating distance between {Airport1} and {Airport2}", airport1, airport2);

            AirportData airportData1;
            AirportData airportData2;

            try
            {
                airportData1 = await _airportService.GetAirportDataAsync(airport1);
                airportData2 = await _airportService.GetAirportDataAsync(airport2);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching airport data");
                return StatusCode(500, "Internal server error while fetching airport data.");
            }

            if (airportData1 == null || airportData2 == null)
            {
                _logger.LogWarning("One or both of the airports were not found: {Airport1}, {Airport2}", airport1, airport2);
                return NotFound("One or both of the airports were not found.");
            }

            var lat1 = airportData1.Location?.Lat;
            var lon1 = airportData1.Location?.Lon;
            var lat2 = airportData2.Location?.Lat;
            var lon2 = airportData2.Location?.Lon;

            if (lat1 == null || lon1 == null || lat2 == null || lon2 == null)
            {
                _logger.LogWarning("Location data for one or both airports is missing: {Airport1}, {Airport2}", airport1, airport2);
                return NotFound("Location data for one or both airports is missing.");
            }

            var distance = CalculateHaversineDistance(lat1.Value, lon1.Value, lat2.Value, lon2.Value);
            _logger.LogInformation("Calculated distance: {Distance} miles", distance);

            // Форматирование ответа
            return Ok(new { distance = Math.Round(distance, 2) });
        }

        private double CalculateHaversineDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 3958.8; // Radius of Earth in miles
            var lat1Rad = DegreesToRadians(lat1);
            var lat2Rad = DegreesToRadians(lat2);
            var deltaLatRad = DegreesToRadians(lat2 - lat1);
            var deltaLonRad = DegreesToRadians(lon2 - lon1);

            var a = Math.Sin(deltaLatRad / 2) * Math.Sin(deltaLatRad / 2) +
                    Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
                    Math.Sin(deltaLonRad / 2) * Math.Sin(deltaLonRad / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }

        private double DegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180;
        }
    }
}
