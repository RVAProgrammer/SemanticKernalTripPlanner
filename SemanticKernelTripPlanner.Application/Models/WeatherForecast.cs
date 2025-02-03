using System.Text.Json.Serialization;

namespace SemanticKernelTripPlanner.Application.Models;

public record WeatherForecast
{
    [JsonPropertyName("date")]
    public DateOnly Date { get; set; }
    [JsonPropertyName("temperatureC")]
    public int TemperatureC { get; set; }
    [JsonPropertyName("temperatureF")]
    public int TemperatureF { get; set; }
    [JsonPropertyName("summary")]
    public string Summary { get; set; }
}