namespace AutoFixtureDemo.DomainObjects
{
    public class WeatherForecast(DateOnly date, Celsius temperatureC, WeatherSummary summary)
    {
        public DateOnly Date { get; private set; } = date;
        public Celsius TemperatureC { get; private set; } = temperatureC;
        public WeatherSummary Summary { get; private set; } = summary;
        public Fahrenheit TemperatureF  => (Fahrenheit)TemperatureC;
    }
}
