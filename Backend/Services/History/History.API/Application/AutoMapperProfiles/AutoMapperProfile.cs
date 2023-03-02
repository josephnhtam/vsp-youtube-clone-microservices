using AutoMapper;
using History.API.Application.DtoModels;
using History.Domain.Models;

namespace History.API.Application.AutoMapperProfiles {
    public class AutoMapperProfile : Profile {

        public AutoMapperProfile () {
            CreateMap<UserProfile, InternalUserProfileDto>();

            CreateMap<Video, VideoDto>()
                .ForMember(x => x.CreatorProfile, x => x.MapFrom<CreatorProfileResolver>())
                .ForMember(x => x.ThumbnailUrl, x => x
                    .ConvertUsing<FileUrlResolver, string>())
                .ForMember(x => x.PreviewThumbnailUrl, x => x
                    .ConvertUsing<FileUrlResolver, string>());

            CreateMap<VideoMetrics, VideoMetricsDto>();

            CreateMap<UserProfile, CreatorProfileDto>().ForMember(x => x.ThumbnailUrl, x => x.ConvertUsing<FileUrlResolver, string>());
        }

    }
}
