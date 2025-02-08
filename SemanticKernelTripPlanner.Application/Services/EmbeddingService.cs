using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Azure;
using Azure.Search.Documents;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Embeddings;
using SemanticKernalTripPlanner.Application.Identity;
using SemanticKernelTripPlanner.Application.Configuration;
using SemanticKernelTripPlanner.Application.Models;

namespace SemanticKernelTripPlanner.Application.Services;

public interface IEmbeddingService
{
    Task<ReadOnlyMemory<float>> GenerateEmbeddings(string text, CancellationToken cancellationToken = default);
    Task ProcessFile(string filePath, CancellationToken cancellationToken = default);
}


[Experimental("SKEXP0010")]
public class EmbeddingService(IOptions<AzureOpenAIConfiguration> azureOpenAiOptions, IOptions<AzureSearchConfiguration> searchConfigurationOptions)  :IEmbeddingService
{
    private readonly AzureOpenAIConfiguration _azureOpenAIConfiguration = azureOpenAiOptions.Value;
    private readonly AzureSearchConfiguration _azureSearchConfiguration = searchConfigurationOptions.Value;

   
    public async Task<ReadOnlyMemory<float>> GenerateEmbeddings(string text, CancellationToken cancellationToken= default)
    {
        var builder = Kernel.CreateBuilder().AddAzureOpenAITextEmbeddingGeneration(_azureOpenAIConfiguration.EmbeddingDeploymentName, _azureOpenAIConfiguration.URI, new BearerTokenCredential(),
            httpClient:new HttpClient(new ProxyOpenAIHandler())).Build();

        var embeddingService = builder.GetRequiredService<ITextEmbeddingGenerationService>();
        var embeddings = await embeddingService.GenerateEmbeddingAsync(text, cancellationToken: cancellationToken);
        return embeddings;
    }

    public async Task ProcessFile(string filePath, CancellationToken cancellationToken = default)
    {
        var builder = Kernel.CreateBuilder().AddAzureOpenAITextEmbeddingGeneration(_azureOpenAIConfiguration.EmbeddingDeploymentName, 
            _azureOpenAIConfiguration.URI, new BearerTokenCredential(),
            httpClient:new HttpClient(new ProxyOpenAIHandler())).Build();

        using var pdfReader = new PdfReader(filePath);
        var doc = new PdfDocument(pdfReader);
        
        var client = new SearchClient(new Uri(_azureSearchConfiguration.URL), "park-index",
            new AzureKeyCredential(_azureSearchConfiguration.Key));

        var pages = new List<ParkIndexItem>();


        var embeddingService = builder.GetRequiredService<ITextEmbeddingGenerationService>();
        for (var i = 1; i <= doc.GetNumberOfPages(); i++)
        {
            var pageText = PdfTextExtractor.GetTextFromPage(doc.GetPage(i));
            var pageEmbeddings = await embeddingService.GenerateEmbeddingAsync(pageText, cancellationToken: cancellationToken);
            
            pages.Add(new ParkIndexItem
            {
                Id = Guid.NewGuid().ToString(),
                Content =pageText,
                ContentVector = pageEmbeddings,
                DocumentName = filePath,
                PageNumber = i
            });
            
        }
        
        doc.Close();
        pdfReader.Close();
        
        
        await client.UploadDocumentsAsync(pages.ToArray(), new IndexDocumentsOptions(), cancellationToken);

    }
} 