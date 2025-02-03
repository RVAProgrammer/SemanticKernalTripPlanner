using System.ComponentModel;
using System.Text.Json;
using Microsoft.SemanticKernel;
using SemanticKernelTripPlanner.Application.Models;

namespace SemanticKernelTripPlanner.Application.Plugins;

public class WeatherPlugin
{
    [KernelFunction("get_weather")]
    [Description("Get a weather forecast for the number of days supplied")]
    [return: Description("An array of weather forcaste")]
    public async Task<List<WeatherForecast>> GetWeatherForecast(int days)
    {
        var httpCient = new HttpClient() { BaseAddress = new Uri("http://localhost:5254/") };

        var result = await httpCient.GetAsync($"/weatherforecast/{days}");
        result.EnsureSuccessStatusCode();

        var raw = await result.Content.ReadAsStringAsync();
        var weather = JsonSerializer.Deserialize<List<WeatherForecast>>(raw);
        return weather ?? [];
    }
}