using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using AutoFixtureDemo.Controllers;
using AutoFixtureDemo.Services;
using System.Collections.Generic;

public class WeatherForecastControllerTests
{
    [Fact]
    public void Get_ReturnsForecastsFromService()
    {
        // Arrange
        var mockLogger = new Mock<ILogger<WeatherForecastController>>();
        var mockService = new Mock<WeatherForecastService>();
        var expectedForecasts = new List<WeatherForecast>
        {
            new WeatherForecast { Date = DateOnly.FromDateTime(System.DateTime.Now), TemperatureC = 20, Summary = "Mild" }
        };
        mockService.Setup(s => s.GetForecasts()).Returns(expectedForecasts);

        var controller = new WeatherForecastController(mockLogger.Object, mockService.Object);

        // Act
        var result = controller.Get();

        // Assert
        Assert.Equal(expectedForecasts, result);
    }
}