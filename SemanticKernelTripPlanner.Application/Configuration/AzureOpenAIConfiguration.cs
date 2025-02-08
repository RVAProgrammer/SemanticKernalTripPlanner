namespace SemanticKernelTripPlanner.Application.Configuration;

public class AzureOpenAIConfiguration
{
    public static string SectionName = "AzureOpenAIConfiguration";
    public string URI { get; set; }
    public string DeploymentName { get; set; }
    public string EmbeddingDeploymentName { get; set; }
}