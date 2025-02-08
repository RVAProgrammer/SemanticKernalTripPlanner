using System.ComponentModel;
using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using Json.More;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Microsoft.SemanticKernel;
using SemanticKernelTripPlanner.Application.Configuration;
using SemanticKernelTripPlanner.Application.Models;
using SemanticKernelTripPlanner.Application.Services;

namespace SemanticKernelTripPlanner.Application.Plugins;

public class SearchTripIndexPlugin(AzureSearchConfiguration _azureSearchConfiguration, IEmbeddingService _embeddingService)
{
    
    [KernelFunction("NationalParkServiceGuideSearch")]
    [Description(
        "Search in the national park service trip guide for information how to to best enjoy National Parks and how to stay safe")]
    public async Task<string> SearchAsync(string query)
    {
        var client = new SearchClient(new Uri(_azureSearchConfiguration.URL), "park-index",
            new AzureKeyCredential(_azureSearchConfiguration.Key));

        var embeddings = await _embeddingService.GenerateEmbeddings(query);
        var results = await client.SearchAsync<ParkIndexItem>(null,
            new SearchOptions
            {
                VectorSearch = new VectorSearchOptions()
                {
                    Queries   = { new VectorizedQuery(embeddings)
                    {
                        KNearestNeighborsCount = 3,
                        Fields = { "ContentVector" }
                        
                    } }
                },
                Select = { "Content" }
            });

        var answer = "";
        await foreach (var result in results.Value.GetResultsAsync())
        {
            answer = result.Document.Content;
            break;
        }
        return answer;
        

    }
}