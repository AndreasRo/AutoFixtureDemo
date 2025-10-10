using Moq;
using Microsoft.Extensions.Logging;
using AutoFixtureDemo.Controllers;
using AutoFixtureDemo.Services;
using AutoFixtureDemo;

public class WeatherForecastControllerTests
{
    [Fact]
    public void Get_ReturnsForecastsFromService()
    {
        // Arrange
        var mockLogger = new Mock<ILogger<WeatherForecastController>>();
        var mockService = new Mock<IWeatherForecastService>();


        var expectedForecasts = new List<WeatherForecast>
        {
            new WeatherForecast { Date = DateOnly.FromDateTime(DateTime.Now), TemperatureC = 20, Summary = "Mild" }
        };
        
        mockService.Setup(s => s.GetForecasts()).Returns(expectedForecasts);

        var controller = new WeatherForecastController(mockLogger.Object, mockService.Object);

        // Act
        var result = controller.Get();

        // Assert
        Assert.Equal(expectedForecasts, result);
    }
}