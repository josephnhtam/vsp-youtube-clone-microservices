using AutoMapper;
using Library.API.Application.DtoModels;
using Library.Domain.Models;

namespace Library.API.Application.AutoMapperProfiles {
    public class CreatorProfileResolver :
        IValueResolver<Video, VideoDto, CreatorProfileDto> {

        public CreatorProfileResolver () {
        }

        public CreatorProfileDto Resolve (Video source, VideoDto destination, CreatorProfileDto destMember, ResolutionContext context) {
            return context.Items.GetValueOrDefault("creator") as CreatorProfileDto ?? new CreatorProfileDto { Id = source.CreatorId };
        }

    }
}
