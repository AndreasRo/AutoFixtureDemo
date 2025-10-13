using AutoFixtureDemo.DomainObjects;

namespace AutoFixtureDemo.Database.Entities;

public class WeatherForecastEntity
{
    public int Id { get; set; }
    public DateOnly Date { get; set; }
    public int TemperatureC { get; set; }
    public WeatherSummary Summary { get; set; }
    public int LocationId { get; set; }
    public LocationEntity Location { get; set; } = null!;
}