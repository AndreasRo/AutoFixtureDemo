using AutoFixtureDemo.Controllers;
using AutoFixtureDemo.DomainObjects;
using AutoFixtureDemo.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace AutoFixtureDemo.UnitTests.Controllers;

public class WeatherForecastControllerTests
{
    private readonly Mock<IWeatherForecastService> _mockService = new();

    [Fact]
    public void Get_ReturnsForecastsFromService()
    {
        // Arrange
        var location = new Location
        {
            City = new Text("SomeCity"),
            Country = new Text("SomeCountry"),
            Forecasts = new List<WeatherForecast>
            {
                new() { Date = DateOnly.FromDateTime(DateTime.Now), TemperatureC = new Celsius(24), Summary = WeatherSummary.Balmy },
                new() { Date = DateOnly.FromDateTime(DateTime.Now.AddDays(1)), TemperatureC = new Celsius(20), Summary = WeatherSummary.Mild },
                new() { Date = DateOnly.FromDateTime(DateTime.Now.AddDays(2)), TemperatureC = new Celsius(40), Summary = WeatherSummary.Sweltering },
                new() { Date = DateOnly.FromDateTime(DateTime.Now.AddDays(3)), TemperatureC = new Celsius(7), Summary = WeatherSummary.Cool },
                new() { Date = DateOnly.FromDateTime(DateTime.Now.AddDays(4)), TemperatureC = new Celsius(6), Summary = WeatherSummary.Cool },
                new() { Date = DateOnly.FromDateTime(DateTime.Now.AddDays(5)), TemperatureC = new Celsius(-12), Summary = WeatherSummary.Freezing },
            }
        };

    _mockService.Setup(s => s.GetForecastsForLocation(It.IsAny<string?>())).Returns(location);

    // create mapper
    var mapperConfig = new AutoMapper.MapperConfiguration(cfg => cfg.AddProfile<Mapping.MappingProfile>(), new LoggerFactory());
    var mapper = mapperConfig.CreateMapper();

    var controller = new WeatherForecastController(_mockService.Object, mapper);

    // Act
    var result = controller.Get("some-location");

    // Assert
    var ok = Assert.IsType<OkObjectResult>(result.Result);
    var dto = Assert.IsType<AutoFixtureDemo.Controllers.DTO.LocationDto>(ok.Value);

    Assert.Equal(location.City.Value, dto.City);
    Assert.Equal(location.Country.Value, dto.Country);
    Assert.Equal(location.Forecasts.Count, dto.Forecasts.Count);
    }
}