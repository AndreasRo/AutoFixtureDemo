using System.Collections.Generic;
using AutoFixtureDemo.DomainObjects;

namespace AutoFixtureDemo.Controllers.DTO
{
    public class LocationDto
    {
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public List<WeatherForecastDto> Forecasts { get; set; } = new List<WeatherForecastDto>();
    }
}
