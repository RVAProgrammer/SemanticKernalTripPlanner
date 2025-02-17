using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using SemanticKernelTripPlanner.Application.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using SemanticKernalTripPlanner.Application.Identity;
using SemanticKernelTripPlanner.Application.DTO;
using SemanticKernelTripPlanner.Application.Plugins;
using SemanticKernelTripPlanner.Application.Services;

namespace SemanticKernelTripPlanner.Application;

public interface ITripPlanner
{
    Task<string> GetTripPlan(TripRequest request);
    Task<string> GetTripPlanWithWeather(TripRequest request);
    Task<string> GetTripPlanWithWeatherRag(TripRequest request);
}

public class TripPlanner(IOptions<AzureOpenAIConfiguration> azureOpenAiOptions, IOptions<AzureSearchConfiguration> azureSearchOptions, IEmbeddingService _embeddingService) : ITripPlanner
{
    private readonly AzureOpenAIConfiguration _azureOpenAIConfiguration = azureOpenAiOptions.Value;
    private readonly AzureSearchConfiguration _azureSearchConfiguration = azureSearchOptions.Value;

    public async Task<string> GetTripPlan(TripRequest request)
    {
        return await Task.FromResult("Lets go");
    }
    
    public async Task<string> GetTripPlanWithWeather(TripRequest request)
    {
        return await Task.FromResult("Lets go");
    }

    public async Task<string> GetTripPlanWithWeatherRag(TripRequest request)
    {
        return await Task.FromResult("Lets go");
    }

}