using Microsoft.Extensions.DependencyInjection;
using Polly;

namespace Application.Utilities {
    public static class HttpClientUtilities {

        public static IHttpClientBuilder AddTransientHttpErrorPolicy (this IHttpClientBuilder builder) {
            return builder.AddTransientHttpErrorPolicy(builder =>
                    builder.WaitAndRetryAsync(6,
                        retryAttempts => TimeSpan.FromSeconds(Math.Pow(2f, retryAttempts))));
        }

    }
}
