using AutoFixtureDemo.DomainObjects;

namespace AutoFixtureDemo.Services
{
    public class WeatherForecastService : IWeatherForecastService
    {
        private static readonly WeatherSummary[] Summaries = new[]
        {
            WeatherSummary.Freezing,
            WeatherSummary.Bracing,
            WeatherSummary.Chilly,
            WeatherSummary.Cool,
            WeatherSummary.Mild,
            WeatherSummary.Warm,
            WeatherSummary.Balmy,
            WeatherSummary.Hot,
            WeatherSummary.Sweltering,
            WeatherSummary.Scorching
        };

        public Location GetForecastsForLocation(string? locationName)
        {
            return new Location
            {
                City = locationName ?? "Unknown",
                Country = "Unknown",
                Forecasts = Enumerable.Range(1, 5).Select(index => new WeatherForecast
                {
                    Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    TemperatureC = new Celsius(Random.Shared.Next(-20, 55)),
                    Summary = Summaries[Random.Shared.Next(Summaries.Length)]
                }).ToList()
            };
        }
    }
    
    public interface IWeatherForecastService
    {
        Location GetForecastsForLocation(string? locationName);
    }
}