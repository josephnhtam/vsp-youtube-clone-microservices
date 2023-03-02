using AutoMapper;
using Microsoft.Extensions.Options;
using VideoStore.API.Application.Configurations;

namespace VideoStore.API.Application.AutoMapperProfiles {
    public class FileUrlResolver : IValueConverter<string, string> {

        private readonly StorageConfiguration _config;

        public FileUrlResolver (IOptions<StorageConfiguration> config) {
            _config = config.Value;
        }

        public string Convert (string sourceMember, ResolutionContext context) {
            return ResolveFileUrl(_config.BaseUri, sourceMember, context);
        }

        public static string ResolveFileUrl (string baseUri, string? resourcePath, ResolutionContext context) {
            if (string.IsNullOrEmpty(resourcePath)) {
                return string.Empty;
            }

            try {
                if (!context.Items.TryGetValue("resolveUrl", out var val) || !(val is bool resolveUrl) || !resolveUrl) {
                    return resourcePath;
                }
            } catch (Exception) {
                return resourcePath;
            }

            if (string.IsNullOrWhiteSpace(baseUri)
                || resourcePath.StartsWith("http://")
                || resourcePath.StartsWith("https://")) {
                return resourcePath;
            } else {
                return new Uri(new Uri(baseUri), resourcePath).ToString();
            }
        }
    }
}