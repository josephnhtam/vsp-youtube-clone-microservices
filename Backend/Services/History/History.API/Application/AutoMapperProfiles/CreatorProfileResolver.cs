using AutoMapper;
using History.API.Application.DtoModels;
using History.Domain.Models;

namespace History.API.Application.AutoMapperProfiles {
    public class CreatorProfileResolver :
        IValueResolver<Video, VideoDto, CreatorProfileDto> {

        public CreatorProfileResolver () {
        }

        public CreatorProfileDto Resolve (Video source, VideoDto destination, CreatorProfileDto destMember, ResolutionContext context) {
            return context.Items.GetValueOrDefault("creator") as CreatorProfileDto ?? new CreatorProfileDto { Id = source.CreatorId };
        }

    }
}
