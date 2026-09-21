using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;

namespace Service.Reports.Client;

internal sealed class ServiceAccessTokenProvider(
    IHttpClientFactory httpClientFactory,
    IConfiguration configuration)
{
    private readonly SemaphoreSlim _lock = new(1, 1);
    private string? _accessToken;
    private DateTimeOffset _expiresAtUtc;

    public async Task<string> GetAsync(CancellationToken cancellationToken)
    {
        if (HasValidToken())
        {
            return _accessToken!;
        }

        await _lock.WaitAsync(cancellationToken);
        try
        {
            if (HasValidToken())
            {
                return _accessToken!;
            }

            var clientId = configuration["Reports:ServiceClient:ClientId"]
                           ?? throw new InvalidOperationException("Reports:ServiceClient:ClientId is not configured.");
            var clientSecret = configuration["Reports:ServiceClient:ClientSecret"]
                               ?? throw new InvalidOperationException("Reports:ServiceClient:ClientSecret is not configured.");

            var content = new FormUrlEncodedContent([
                new KeyValuePair<string, string>("grant_type", "client_credentials"),
                new KeyValuePair<string, string>("client_id", clientId),
                new KeyValuePair<string, string>("client_secret", clientSecret)
            ]);

            using var response = await httpClientFactory.CreateClient("ReportsKeycloak")
                .PostAsync("protocol/openid-connect/token", content, cancellationToken);
            response.EnsureSuccessStatusCode();

            var token = await response.Content.ReadFromJsonAsync<TokenResponse>(cancellationToken)
                        ?? throw new InvalidOperationException("Keycloak returned an empty access token response.");

            _accessToken = token.AccessToken;
            _expiresAtUtc = DateTimeOffset.UtcNow.AddSeconds(token.ExpiresIn);
            return _accessToken;
        }
        finally
        {
            _lock.Release();
        }
    }

    private bool HasValidToken() =>
        !string.IsNullOrWhiteSpace(_accessToken) && DateTimeOffset.UtcNow < _expiresAtUtc.AddMinutes(-1);

    private sealed record TokenResponse(
        [property: JsonPropertyName("access_token")] string AccessToken,
        [property: JsonPropertyName("expires_in")] int ExpiresIn);
}
