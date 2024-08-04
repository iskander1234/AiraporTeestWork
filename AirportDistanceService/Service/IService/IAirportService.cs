using AirportDistanceService.Models;

namespace AirportDistanceService.Service.IService;

public interface IAirportService
{
    Task<AirportData> GetAirportDataAsync(string iataCode);
}