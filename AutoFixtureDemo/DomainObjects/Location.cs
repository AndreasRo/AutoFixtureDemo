namespace AutoFixtureDemo.DomainObjects
{
    public class Location
    {
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public List<WeatherForecast> Forecasts { get; set; } = new List<WeatherForecast>();
    }
}
