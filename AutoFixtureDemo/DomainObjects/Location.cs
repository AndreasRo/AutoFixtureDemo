namespace AutoFixtureDemo.DomainObjects
{
    public class Location
    {
    public Text City { get; set; } = new Text(string.Empty);
    public Text Country { get; set; } = new Text(string.Empty);
        public List<WeatherForecast> Forecasts { get; set; } = new List<WeatherForecast>();
    }
}
