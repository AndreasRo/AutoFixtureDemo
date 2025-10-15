using AutoFixtureDemo.Database.Entities;
using AutoFixtureDemo.DomainObjects;

namespace AutoFixtureDemo.Database;

public interface ILocationRepository
{
    void AddLocation(LocationEntity location);
    LocationEntity? GetLocationByName(string cityName);
}

public class LocationRepository : ILocationRepository
{
    private readonly List<LocationEntity> _locations = [];

    public void AddLocation(LocationEntity location)
    {
        _locations.Add(location);
    }

    public LocationEntity GetLocationByName(string cityName)
    {
        return _locations.FirstOrDefault(loc => loc.City.Equals(cityName, StringComparison.OrdinalIgnoreCase)) ?? GetDummy(cityName);
    }

    private static LocationEntity GetDummy(string? locationName)
    {
        var summaries = Enum.GetValues<WeatherSummary>();
        return new LocationEntity
        {
            City = locationName ?? "Unknown",
            Country = "Unknown",
            Forecasts = [.. Enumerable.Range(1, 5).Select(index => new WeatherForecastEntity
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = summaries[Random.Shared.Next(summaries.Length)]
            })]
        };
    }
}