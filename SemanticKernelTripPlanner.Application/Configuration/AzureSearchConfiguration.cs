namespace SemanticKernelTripPlanner.Application.Configuration;

public class AzureSearchConfiguration
{
    public const string SectionName = "AzureSearchConfiguration";
    public string URL { get; set; }
    public string Key { get; set; }
}