using Xunit;
using Moq;
using AutoFixtureDemo.Controllers;
using AutoFixtureDemo.Services;
using AutoFixtureDemo.DomainObjects;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

public class WeatherForecastControllerTests
{
    [Fact]
    public void Get_ReturnsForecastsFromService()
    {
        // Arrange
        var mockService = new Mock<IWeatherForecastService>();
        var expectedForecasts = new List<WeatherForecast>
        {
            new WeatherForecast { Date = DateOnly.FromDateTime(System.DateTime.Now), TemperatureC = new Celsius(20), Summary = WeatherSummary.Mild }
        };
        mockService.Setup(s => s.GetForecasts(It.IsAny<string?>())).Returns(expectedForecasts);

        var controller = new WeatherForecastController(mockService.Object);

        // Act
        var result = controller.Get("loc");

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(expectedForecasts, ok.Value);
    }
}