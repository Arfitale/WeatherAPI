using WeatherApi.Models;

namespace WeatherApi.Services;

public interface IWeatherService
{
    Task<WeatherResponse> GetWeatherAsync(string location);
}
