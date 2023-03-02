using AutoMapper;
using Community.API.Application.DtoModels;
using Community.Domain.Models;

namespace Community.API.Application.AutoMapperProfiles {
    public class AutoMapperProfile : Profile {

        public AutoMapperProfile () {
            CreateMap<VideoComment, VideoCommentDto>();
            CreateMap<UserProfile, UserProfileDto>().ForMember(x => x.ThumbnailUrl, x => x.ConvertUsing<FileUrlResolver, string>());
        }

    }
}
