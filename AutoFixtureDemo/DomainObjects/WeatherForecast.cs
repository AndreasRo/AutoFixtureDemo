namespace AutoFixtureDemo.DomainObjects
{
    public class WeatherForecast
    {
        public DateOnly Date { get;  set; }
        public Celsius TemperatureC { get; set; }
        public WeatherSummary Summary { get; set; } 
        public Fahrenheit TemperatureF  => (Fahrenheit)TemperatureC;
    }
}
