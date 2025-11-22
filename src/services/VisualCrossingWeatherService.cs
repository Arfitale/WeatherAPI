using System.Text.Json;
using DotNetEnv;
using WeatherApi.Models;

namespace WeatherApi.Services;

public class VisualCrossingWeatherService : IWeatherService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public VisualCrossingWeatherService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiKey =
            Env.GetString("WEATHER_API_KEYS") ?? configuration["WeatherApi:ApiKey"] ?? string.Empty;
        _httpClient.BaseAddress = new Uri(
            "https://weather.visualcrossing.com/VisualCrossingWebServices/rest/services/timeline/"
        );
    }

    public async Task<WeatherResponse> GetWeatherAsync(string location)
    {
        Console.WriteLine($"Fetching weather data for location: {location}");
        var response = await _httpClient.GetAsync(
            $"{location}?unitGroup=metric&key={_apiKey}&contentType=json"
        );

        Console.WriteLine($"Received response: {response.StatusCode}");

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"Failed to fetch weather data for location: {location}");
            throw new HttpRequestException("Failed to fetch weather data.");
        }

        var json = await response.Content.ReadAsStringAsync();
        var visualCrossingData = JsonSerializer.Deserialize<VisualCrossingResponse>(json);

        if (visualCrossingData == null)
        {
            Console.WriteLine("Failed to deserialize weather data.");
            throw new Exception("Failed to deserialize weather data.");
        }

        // return MapToWeatherResponse(visualCrossingData);
        Console.WriteLine("Successfully fetched and deserialized weather data.");
        return MapToWeatherResponse(visualCrossingData);
    }

    private WeatherResponse MapToWeatherResponse(VisualCrossingResponse visualCrossingData)
    {
        return new WeatherResponse
        {
            Location = visualCrossingData.address,
            Address = visualCrossingData.resolvedAddress,
            Country = visualCrossingData.resolvedAddress.Split(',').Last().Trim(),
            Latitude = visualCrossingData.latitude,
            Longitude = visualCrossingData.longitude,
            DailyForecasts = visualCrossingData
                .days.Select(day => new DailyForecast
                {
                    Date = DateTime.Parse(day.datetime),
                    Condition = day.conditions,
                    Humidity = day.humidity,
                    Temperature = day.temp,
                    Icon = day.icon,
                    WindSpeed = day.windspeed,
                })
                .ToList(),
        };
    }
}
