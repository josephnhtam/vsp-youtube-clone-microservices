using Application.Identities.Configurations;
using IdentityModel.Client;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Application.Identities.Handlers {
    public class BearerTokenHandler : DelegatingHandler {

        private readonly ClientCredentials _clientCredentials;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMemoryCache _memoryCache;
        private readonly IHostingEnvironment _env;
        private readonly ILogger<BearerTokenHandler> _logger;

        public BearerTokenHandler (IOptions<ClientCredentials> clientCredentials, IHttpClientFactory httpClientFactory, IMemoryCache memoryCache, IHostingEnvironment env, ILogger<BearerTokenHandler> logger) {
            _clientCredentials = clientCredentials.Value;
            _httpClientFactory = httpClientFactory;
            _memoryCache = memoryCache;
            _env = env;
            _logger = logger;
        }

        protected override HttpResponseMessage Send (HttpRequestMessage request, CancellationToken cancellationToken) {
            var accessToken = GetAccessToken().ConfigureAwait(false).GetAwaiter().GetResult();

            if (accessToken != null) {
                request.SetBearerToken(accessToken);
            }

            return base.Send(request, cancellationToken);
        }

        protected override async Task<HttpResponseMessage> SendAsync (HttpRequestMessage request, CancellationToken cancellationToken) {
            var accessToken = await GetAccessToken();

            if (!string.IsNullOrEmpty(accessToken)) {
                request.SetBearerToken(accessToken);
            }

            return await base.SendAsync(request, cancellationToken);
        }

        private async Task<string?> GetAccessToken () {
            string cacheId = $"Token:{_clientCredentials.ClientId}";

            if (_memoryCache.TryGetValue<string>(cacheId, out var token)) {
                return token;
            } else {
                using var client = _httpClientFactory.CreateClient("BearerTokenHandlerClient");

                var discovery = await client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest {
                    Address = _clientCredentials.Authority,
                    Policy = new DiscoveryPolicy {
                        RequireHttps = !_env.IsDevelopment()
                    }
                });

                if (discovery.IsError) {
                    _logger.LogError("Failed to get discovery document from {Authority}", _clientCredentials.Authority);
                    return null;
                }

                var tokenResponse = await client.RequestClientCredentialsTokenAsync(
                    new ClientCredentialsTokenRequest {
                        Address = discovery.TokenEndpoint,
                        ClientId = _clientCredentials.ClientId,
                        ClientSecret = _clientCredentials.ClientSecret,
                        Scope = _clientCredentials.Scope
                    }
                );

                if (tokenResponse.IsError || string.IsNullOrEmpty(tokenResponse.AccessToken)) {
                    _logger.LogError("Failed to get token response from {Address}", discovery.TokenEndpoint);
                    return null;
                }

                _logger.LogInformation("Obtained acces Token: {AccessToken}", tokenResponse.AccessToken);

                _logger.LogInformation("Cache token response for {ClientId}, expired in {ExpiredIn}",
                    _clientCredentials.ClientId, tokenResponse.ExpiresIn);

                using (var entry = _memoryCache.CreateEntry(cacheId)) {
                    entry.SetValue(tokenResponse.AccessToken);
                    entry.SetAbsoluteExpiration(TimeSpan.FromSeconds(tokenResponse.ExpiresIn - 60));
                    entry.SetPriority(CacheItemPriority.NeverRemove);
                }

                return tokenResponse.AccessToken;
            }
        }

    }
}
