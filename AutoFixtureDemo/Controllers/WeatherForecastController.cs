using AutoFixtureDemo.Services;
using AutoFixtureDemo.Controllers.DTO;
using Microsoft.AspNetCore.Mvc;

namespace AutoFixtureDemo.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController(
    IWeatherForecastService weatherForecastService,
    AutoMapper.IMapper mapper)
    : ControllerBase
{
    [HttpGet(Name = "GetWeatherForecast")]
    public ActionResult<LocationDto> Get([FromQuery] string? location = null)
    {
        if (location == null)
        {
            return BadRequest("Location parameter is required.");
        }

        var domainLocation = weatherForecastService.GetForecastsForLocation(location);

        var dto = mapper.Map<LocationDto>(domainLocation);

        return Ok(dto);
    }
}
