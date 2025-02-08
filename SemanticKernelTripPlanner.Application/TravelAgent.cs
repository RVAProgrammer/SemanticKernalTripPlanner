using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Agents.Chat;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using SemanticKernalTripPlanner.Application.Identity;
using SemanticKernelTripPlanner.Application.Configuration;
using SemanticKernelTripPlanner.Application.Plugins;
using SemanticKernelTripPlanner.Application.Services;

namespace SemanticKernelTripPlanner.Application;

public interface ITravelAgent
{
    void Init();
    Task<string> PlanTrip(string tripRequest);
}

[Experimental("SKEXP0110")]
public class TravelAgent(
    IOptions<AzureOpenAIConfiguration> azureOpenAiOptions,
    IOptions<AzureSearchConfiguration> azureSearchOptions,
    IEmbeddingService _embeddingService) : ITravelAgent
{
    private readonly AzureOpenAIConfiguration _azureOpenAIConfiguration = azureOpenAiOptions.Value;
    private readonly AzureSearchConfiguration _azureSearchConfiguration = azureSearchOptions.Value;

    
    private string _travelAgentInstructions =
        "You are a travel agent who specializes in planning outdoor adventures for people. Work with the the weatherman, outfitter, and parkranger to help plan the trip.  " +
        "Your final deliverable will be a weather report, provided by the weatherman,  for each day of the trip along with recommended gear from the outfitters.  Provide interesting facts about the location the person is traveling to. " +
        "If the trip is to a National Park check with the park ranger in order to get more details about the trip.  Once you have heard back from everyone compile everything into the final trip report.  If no specific dates are given assume they are leaving tomorrow.";

    private string _weatherManInstructions =
        "You are a weatherman and you report the weather for however many days the travel agent needs.  If the travel agent doesn't specify the number of days are specified your default response is to give the next 5 days of weather, use the weather plug in to fetch the weather.";

    private string _outfitterInstructions =
        "You are an expert in gearing adventurers for outstanding trips.  Tell the travel agent the needed gear for every eventuality to make sure the adventure is safe and the members of the party are comfortable." +
        "It is important for you to know how long the trip is and what the weather report is going to be for the duration of the trip.  Where the adventure takes place is important too to help plan the right gear";

    private string _parkRangerInstructions =
        "You are a United States National Park Ranger.  You are an expert in planning any trip inside a national park.  You have many resources at your disposal to help the travel agent plan a trip in the park such as the National Park Service Trip planning guide." +
        "Help adventurers and tourists alike have the best time they can in the National Park.  If the travel agent asks about a trip not inside a park do not provide any information as that is outside of your expertise.";

    private ChatCompletionAgent _travelAgentAgent;
    private ChatCompletionAgent _weatherManAgent;
    private ChatCompletionAgent _outfitterAgent;
    private ChatCompletionAgent _parkRangerAgent;
    
    
    public void Init()
    {
        var kernel = Kernel.CreateBuilder().AddAzureOpenAIChatCompletion(_azureOpenAIConfiguration.DeploymentName,
            _azureOpenAIConfiguration.URI, new BearerTokenCredential(),
            httpClient: new HttpClient(new ProxyOpenAIHandler()));
        
        kernel.Plugins.AddFromType<WeatherPlugin>("Weather");
        
        kernel.Plugins.AddFromObject(new SearchTripIndexPlugin(_azureSearchConfiguration, _embeddingService)); 
        
        var builder = kernel.Build();
        var executionSettings = new AzureOpenAIPromptExecutionSettings()
            { ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions };

        
        _travelAgentAgent= new ChatCompletionAgent { Instructions = _travelAgentInstructions, Name = "TravelAgent", Kernel = builder };
        _outfitterAgent = new ChatCompletionAgent { Instructions = _outfitterInstructions, Name = "Outfitter", Kernel = builder, Arguments  = new KernelArguments(executionSettings) };
        _weatherManAgent = new ChatCompletionAgent { Instructions = _weatherManInstructions, Name = "WeatherMan", Kernel = builder , Arguments  = new KernelArguments(executionSettings)};
        _parkRangerAgent = new ChatCompletionAgent {Instructions = _parkRangerInstructions, Name = "ParkRanger", Kernel = builder, Arguments  = new KernelArguments(executionSettings) };
    }

    public async Task<string?> PlanTrip(string tripRequest)
    {
        var chat = new AgentGroupChat(_travelAgentAgent, _weatherManAgent, _outfitterAgent, _parkRangerAgent)
        {
            ExecutionSettings =
            {
                TerminationStrategy = { MaximumIterations = 5 }
            }
        };

        chat.AddChatMessage(new ChatMessageContent(AuthorRole.User, tripRequest));
        Console.WriteLine($"# {AuthorRole.User}: '{tripRequest}'");

        await foreach (var content in chat.InvokeAsync())
        {
            Console.WriteLine($"# {content.Role} - {content.AuthorName ?? "*"}: '{content.Content}'");
        }

        var mostRecentMessage = await chat.GetChatMessagesAsync().FirstOrDefaultAsync();

        return mostRecentMessage == null ? "" : mostRecentMessage.Content;
    }


}