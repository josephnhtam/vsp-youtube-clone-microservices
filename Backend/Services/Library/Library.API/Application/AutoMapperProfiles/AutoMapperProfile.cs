using AutoMapper;
using Library.API.Application.DtoModels;
using Library.Domain.Models;

namespace Library.API.Application.AutoMapperProfiles {
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
            CreateMap<UserProfile, UserProfileDto>().ForMember(x => x.ThumbnailUrl, x => x.ConvertUsing<FileUrlResolver, string>());

            CreateMap<Playlist, SimplePlaylistDto>();
            CreateMap<Playlist, SimplePlaylistInfoDto>();

            CreateMap<PlaylistRef, GetPlaylistRefResponseDto>();
        }

    }
}
