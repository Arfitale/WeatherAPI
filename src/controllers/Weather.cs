namespace WeatherApi.Controllers;

using DotNetEnv;
using Microsoft.AspNetCore.Mvc;
using WeatherApi.Models;
using WeatherApi.Services;

[ApiController]
[Route("api/[controller]")]
public class WeatherController(IWeatherService weatherService) : ControllerBase
{
    private readonly IWeatherService _weatherService = weatherService;

    [HttpGet("{location}")]
    public async Task<ActionResult<WeatherResponse>> GetWeather(string location)
    {
        try
        {
            var weather = await _weatherService.GetWeatherAsync(location);
            Console.WriteLine($"Returning weather data for location: {location}");
            return Ok(weather);
        }
        catch (HttpRequestException ex)
        {
            return NotFound($"Weather data for location '{location}' not found. {ex.Message}");
        }
        catch (Exception)
        {
            Console.WriteLine("An error occurred while processing the request.");
            return StatusCode(500, "Internal server error");
        }
    }
}
