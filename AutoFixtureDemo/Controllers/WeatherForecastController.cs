using AutoFixtureDemo.DomainObjects;
using System.Collections.Generic;
using AutoFixtureDemo.Services;
using Microsoft.AspNetCore.Mvc;

namespace AutoFixtureDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly IWeatherForecastService _weatherForecastService;

        public WeatherForecastController(
            IWeatherForecastService weatherForecastService)
        {
            _weatherForecastService = weatherForecastService;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public ActionResult<IEnumerable<WeatherForecast>> Get([FromQuery] string? location = null)
        {
            if (location == null)
            {
                return BadRequest("Location parameter is required.");
            }
            return Ok(_weatherForecastService.GetForecastsForLocation(location));
        }
    }
}
