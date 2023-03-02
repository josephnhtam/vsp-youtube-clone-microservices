using AutoMapper;
using VideoManager.API.Application.DtoModels;
using VideoManager.API.Commands;
using VideoManager.Domain.Models;

namespace VideoManager.API.Application.AutoMapperProfiles {
    public class AutoMapperProfile : Profile {

        public AutoMapperProfile () {
            CreateMap<UserProfile, InternalUserProfileDto>();

            CreateMap<Video, VideoDto>()
                .ForMember(x => x.OriginalVideoUrl, x => x.ConvertUsing<FileUrlResolver, string>())
                .AfterMap((video, videoDto) => {
                    videoDto.Thumbnails = videoDto.Thumbnails.OrderBy(x => {
                        if (int.TryParse(x.Label, out var order)) {
                            return order;
                        }
                        return x.Label.GetHashCode();
                    });

                    videoDto.Videos = videoDto.Videos.OrderBy(x => x.Height);
                });

            CreateMap<ProcessedVideo, ProcessedVideoDto>().ForMember(x => x.Url, x => x.ConvertUsing<FileUrlResolver, string>());
            CreateMap<VideoThumbnail, VideoThumbnailDto>().ForMember(x => x.Url, x => x.ConvertUsing<FileUrlResolver, string>());
            CreateMap<VideoMetrics, VideoMetricsDto>();

            CreateMap<SetVideoBasicInfoRequestDto, SetVideoBasicInfoCommand>();
            CreateMap<SetVideoVisibilityInfoRequestDto, SetVideoVisibilityInfoCommand>();
        }

    }
}
