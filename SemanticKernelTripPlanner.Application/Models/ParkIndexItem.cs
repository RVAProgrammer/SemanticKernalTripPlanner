using System.Text.Json.Serialization;

namespace SemanticKernelTripPlanner.Application.Models;

public class ParkIndexItem
{
    [JsonPropertyName("id")]
    public string Id { get; set; }
    public string DocumentName { get; set; }
    public string Content { get; set; }
    public ReadOnlyMemory<float> ContentVector { get; set; }
    public int PageNumber { get; set; }
}