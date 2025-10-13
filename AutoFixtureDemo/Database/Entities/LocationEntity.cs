namespace AutoFixtureDemo.Database.Entities;

public class LocationEntity
{
    public int Id { get; set; }
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public List<WeatherForecastEntity> Forecasts { get; set; } = [];
}
