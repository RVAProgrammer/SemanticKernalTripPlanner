using Azure.Core;

namespace SemanticKernalTripPlanner.Application.Identity;

public class BearerTokenCredential() :TokenCredential
{
    private AccessToken _accessToken;

    public override async ValueTask<AccessToken> GetTokenAsync(TokenRequestContext requestContext, CancellationToken cancellationToken)
    {
        if (_accessToken.ExpiresOn - DateTimeOffset.UtcNow < TimeSpan.FromMinutes(5))
        {
            _accessToken = await AccessTokenHelper.GetAccessTokenAsync();
        }

        return _accessToken;
    }

    public override AccessToken GetToken(TokenRequestContext requestContext, CancellationToken cancellationToken)
    {
        return GetTokenAsync(requestContext, cancellationToken).Result;
    }
}