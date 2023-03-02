using AutoMapper;
using Microsoft.Extensions.Options;
using Users.API.Application.Configurations;
using Users.Domain.Models;

namespace Users.API.Application.AutoMapperProfiles {
    public class ImageFileResolver :
        IValueConverter<ImageFile?, string?> {

        private readonly StorageConfiguration _config;

        public ImageFileResolver (IOptions<StorageConfiguration> config) {
            _config = config.Value;
        }

        public string? Convert (ImageFile? sourceMember, ResolutionContext context) {
            return FileUrlResolver.ResolveFileUrl(_config.BaseUri, sourceMember?.Url, context);
        }

    }
}
