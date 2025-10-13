using AutoFixtureDemo.DomainObjects;

namespace AutoFixtureDemo.Controllers.DTO
{
    public class WeatherForecastDto
    {
        public DateOnly Date { get; set; }
        public int TemperatureC { get; set; }
        public int TemperatureF { get; set; }
        public WeatherSummary Summary { get; set; }
    }
}
