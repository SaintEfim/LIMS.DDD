using Microsoft.AspNetCore.Http;

namespace Service.Reports.Client;

internal sealed class UserAccessTokenHandler(
    IHttpContextAccessor httpContextAccessor,
    ServiceAccessTokenProvider serviceAccessTokenProvider) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var authorization = httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();

        if (string.IsNullOrWhiteSpace(authorization))
        {
            authorization = $"Bearer {await serviceAccessTokenProvider.GetAsync(cancellationToken)}";
        }

        request.Headers.TryAddWithoutValidation("Authorization", authorization);
        return await base.SendAsync(request, cancellationToken);
    }
}
