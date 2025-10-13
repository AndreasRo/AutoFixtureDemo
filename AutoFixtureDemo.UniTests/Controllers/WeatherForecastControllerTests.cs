using Xunit;
using Moq;
using AutoFixtureDemo.Controllers;
using AutoFixtureDemo.Services;
using AutoFixtureDemo.DomainObjects;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

public class WeatherForecastControllerTests
{
    private readonly Mock<IWeatherForecastService> _mockService;

    public WeatherForecastControllerTests()
    {
        _mockService = new Mock<IWeatherForecastService>();
    }

    [Fact]
    public void Get_ReturnsForecastsFromService()
    {
        // Arrange
        var location = new Location
        {
            City = "SomeCity",
            Country = "SomeCountry",
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

        var controller = new WeatherForecastController(_mockService.Object);

        // Act
        var result = controller.Get("some-location");

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(location, ok.Value);
    }
}