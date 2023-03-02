using AutoMapper;
using Search.API.Application.DtoModels;
using Search.Domain.Models;

namespace Search.API.Application.AutoMapperProfiles {
    public class AutoMapperProfile : Profile {

        public AutoMapperProfile () {
            CreateMap<UserProfile, InternalUserProfileDto>().ReverseMap();
            CreateMap<VideoMetrics, VideoMetricsDto>().ReverseMap();

            CreateMap<Video, VideoDto>()
               .ForMember(x => x.ThumbnailUrl, x => x.ConvertUsing<FileUrlResolver, string>())
               .ForMember(x => x.PreviewThumbnailUrl, x => x.ConvertUsing<FileUrlResolver, string>());

            CreateMap<UserProfile, CreatorProfileDto>().ForMember(x => x.ThumbnailUrl, x => x.ConvertUsing<FileUrlResolver, string>());
        }

    }
}
