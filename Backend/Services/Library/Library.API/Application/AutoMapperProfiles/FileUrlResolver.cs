using AutoMapper;
using Library.API.Application.Configurations;
using Microsoft.Extensions.Options;

namespace Library.API.Application.AutoMapperProfiles {
    public class FileUrlResolver : IValueConverter<string, string> {

        private readonly StorageConfiguration _config;

        public FileUrlResolver (IOptions<StorageConfiguration> config) {
            _config = config.Value;
        }

        public string Convert (string sourceMember, ResolutionContext context) {
            try {
                if (!context.Items.TryGetValue("resolveUrl", out var val) || !(val is bool resolveUrl) || !resolveUrl) {
                    return ResolveFileUrl(sourceMember, false);
                }
            } catch (Exception) {
                return ResolveFileUrl(sourceMember, false);
            }

            return ResolveFileUrl(sourceMember, true);
        }

        public string ResolveFileUrl (string? resourcePath, bool resolveUrl) {
            if (string.IsNullOrEmpty(resourcePath)) {
                return string.Empty;
            }

            if (!resolveUrl) {
                return resourcePath;
            }

            string baseUri = _config.BaseUri;

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