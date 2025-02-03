using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using SemanticKernelTripPlanner.Application.Configuration;
using Microsoft.Extensions.Options;
using SemanticKernalTripPlanner.Application.Identity;
using SemanticKernelTripPlanner.Application.DTO;

namespace SemanticKernelTripPlanner.Application;

public interface ITripPlanner
{
    Task<string> GetTripPlan(TripRequest request);
}

public class TripPlanner:ITripPlanner
{
    private readonly AzureOpenAIConfiguration _azureOpenAIConfiguration;
    public TripPlanner(IOptions<AzureOpenAIConfiguration> azureOpenAIOptions)
    {
        _azureOpenAIConfiguration = azureOpenAIOptions.Value;
    }
    
    public async Task<string> GetTripPlan(TripRequest request)
    {
        var builder = Kernel.CreateBuilder().AddAzureOpenAIChatCompletion(_azureOpenAIConfiguration.DeploymentName, _azureOpenAIConfiguration.URI, new BearerTokenCredential(),
            httpClient:new HttpClient(new ProxyOpenAIHandler())).Build();

        var chatService = builder.GetRequiredService<IChatCompletionService>();
        
        var chatHistory = new ChatHistory();
        chatHistory.AddSystemMessage("You are an expert in camping equipment and planning.  Users come to you to help plan out what they need " +
                                     "to bring on back packing trips. Based on the weather alerts in the area recommend appropriate" +
                                     " clothing and supplies hikers should take with them.  If they mention where they are going on the trip, provide some details about the area");
        
        chatHistory.AddUserMessage(request.TripDescription);
        var response = await chatService.GetChatMessageContentsAsync(chatHistory);

        return response.FirstOrDefault().Content;


    }

}