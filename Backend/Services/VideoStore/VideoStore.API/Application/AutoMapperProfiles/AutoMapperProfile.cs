using AutoMapper;
using VideoStore.API.Application.DtoModels;
using VideoStore.Domain.Models;

namespace VideoStore.API.Application.AutoMapperProfiles {
    public class AutoMapperProfile : Profile {

        public AutoMapperProfile () {
            CreateMap<ProcessedVideo, ProcessedVideoDto>().ForMember(x => x.Url, x => x.ConvertUsing<FileUrlResolver, string>());
            CreateMap<Video, VideoDto>()
                .ForMember(x => x.ThumbnailUrl, x => x.ConvertUsing<FileUrlResolver, string>())
                .ForMember(x => x.PreviewThumbnailUrl, x => x.ConvertUsing<FileUrlResolver, string>());
            CreateMap<VideoMetrics, VideoMetricsDto>();

            CreateMap<UserProfile, CreatorProfileDto>().ForMember(x => x.ThumbnailUrl, x => x.ConvertUsing<FileUrlResolver, string>());
        }

    }
}
