namespace AutoFixtureDemo.DomainObjects
{
    public class Location(Text city, Text country, List<WeatherForecast> forecasts)
    {
        public Text City { get; private set; } = city;
        public Text Country { get; private set; } = country;
        public List<WeatherForecast> Forecasts { get; private set; } = forecasts;
    }
}
