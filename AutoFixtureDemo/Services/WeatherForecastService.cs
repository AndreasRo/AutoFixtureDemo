using AutoFixtureDemo.DomainObjects;
using AutoFixtureDemo.Database;
using AutoMapper;

namespace AutoFixtureDemo.Services
{

    public class WeatherForecastService(ILocationRepository locationRepository, IMapper mapper) : IWeatherForecastService
    {
        public Location? GetForecastsForLocation(string? locationName)
        {
            var locationEntity = locationRepository.GetLocationByName(locationName ?? string.Empty);
            return locationEntity == null ? null : mapper.Map<Location>(locationEntity);
        }
    }
    
    public interface IWeatherForecastService
    {
        Location? GetForecastsForLocation(string? locationName);
    }
}