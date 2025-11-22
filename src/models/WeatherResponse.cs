namespace WeatherApi.Models;

public class WeatherResponse
{
    public string Location { get; set; }
    public string Country { get; set; }
    public string Address { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public List<DailyForecast> DailyForecasts { get; set; }
}

public class DailyForecast
{
    public DateTime Date { get; set; }
    public double Temperature { get; set; }
    public string Condition { get; set; }
    public string Icon { get; set; }
    public double WindSpeed { get; set; }
    public double Humidity { get; set; }
}
