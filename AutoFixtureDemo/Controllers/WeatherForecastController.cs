using AutoFixtureDemo.DomainObjects;
using System.Collections.Generic;
using System.Linq;
using AutoFixtureDemo.Services;
using AutoFixtureDemo.Controllers.DTO;
using Microsoft.AspNetCore.Mvc;

namespace AutoFixtureDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly IWeatherForecastService _weatherForecastService;
        private readonly AutoMapper.IMapper _mapper;

        public WeatherForecastController(
            IWeatherForecastService weatherForecastService,
            AutoMapper.IMapper mapper)
        {
            _weatherForecastService = weatherForecastService;
            _mapper = mapper;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public ActionResult<LocationDto> Get([FromQuery] string? location = null)
        {
            if (location == null)
            {
                return BadRequest("Location parameter is required.");
            }

            var domainLocation = _weatherForecastService.GetForecastsForLocation(location);

            var dto = _mapper.Map<LocationDto>(domainLocation);

            return Ok(dto);
        }
    }
}
