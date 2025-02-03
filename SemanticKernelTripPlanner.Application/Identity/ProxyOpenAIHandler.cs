namespace SemanticKernalTripPlanner.Application.Identity;

public class ProxyOpenAIHandler : HttpClientHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (request.RequestUri != null && request.RequestUri.Host.Equals("", StringComparison.OrdinalIgnoreCase))
        {
            var path = request.RequestUri.PathAndQuery.Replace("/openai", "");
            request.RequestUri = new Uri($"/{path}");
        }
        return base.SendAsync(request, cancellationToken);
    }
}
