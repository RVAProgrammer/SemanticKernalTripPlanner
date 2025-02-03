using Azure.Core;
using Azure.Identity;
using Microsoft.Identity.Client;

namespace SemanticKernalTripPlanner.Application.Identity;


public static class AccessTokenHelper
{
    public static async Task<AccessToken> GetAccessTokenAsync(string clientId, string clientSecret, string tenantId,
        string[] scopes)
    {
        var app = ConfidentialClientApplicationBuilder.Create(clientId)
            .WithClientSecret(clientSecret)
            .WithAuthority(new Uri($"https://login.microsoftonline.com/{tenantId}"))
            .Build();

        var result = await app.AcquireTokenForClient(scopes).ExecuteAsync();
        var accessToken = new AccessToken(result.AccessToken, result.ExpiresOn);
        return accessToken;

    }
    
    public static async Task<AccessToken> GetAccessTokenAsync()
    {
        var tokenCred = new DefaultAzureCredential();
        var context = new TokenRequestContext(new[] { "api://ailab/Model.Access" });
        return await tokenCred.GetTokenAsync(context);
    }
}