namespace WeatherApi.Controllers;

using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using WeatherApi.Models;
using WeatherApi.Services;

[ApiController]
[EnableRateLimiting("global")]
[Route("api/[controller]")]
public class WeatherController(IWeatherService weatherService) : ControllerBase
{
    private readonly IWeatherService _weatherService = weatherService;

    [HttpGet("{location}")]
    public async Task<ActionResult<WeatherResponse>> GetWeather(string location)
    {
        try
        {
            var cacheKey = $"weather_:{location.ToLower()}";
            var redis = new Redis();
            var cachedData = await redis.GetCacheValueAsync(cacheKey);

            if (cachedData != null)
            {
                Console.WriteLine($"Returning cached weather data for location: {location}");
                var data = JsonSerializer.Deserialize<WeatherResponse>(cachedData!);
                return Ok(data);
            }

            var weather = await _weatherService.GetWeatherAsync(location);

            var serializedData = JsonSerializer.Serialize(weather);
            Console.WriteLine("Storing weather data in cache.");
            await redis.SetCacheValueAsync(cacheKey, serializedData, TimeSpan.FromMinutes(10));

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
