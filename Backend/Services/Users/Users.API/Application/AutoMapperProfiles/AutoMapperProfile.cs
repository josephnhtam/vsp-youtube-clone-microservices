using AutoMapper;
using Users.API.Application.DtoModels;
using Users.Domain.Models;

namespace Users.API.Application.AutoMapperProfiles {
    public class AutoMapperProfile : Profile {

        public AutoMapperProfile () {
            CreateMap<UserProfile, InternalUserProfileDto>()
                .ForMember(x => x.ThumbnailUrl, x => x.ConvertUsing<ImageFileResolver, ImageFile?>(y => y.Thumbnail));

            CreateMap<UserProfile, UserProfileDto>()
                .ForMember(x => x.ThumbnailUrl, x => x.ConvertUsing<ImageFileResolver, ImageFile?>(y => y.Thumbnail));

            CreateMap<UserProfile, PrivateUserProfileDto>()
                .ForMember(x => x.ThumbnailUrl, x => x.ConvertUsing<ImageFileResolver, ImageFile?>(y => y.Thumbnail));

            CreateMap<UserProfile, DetailedUserProfileDto>()
                .ForMember(x => x.ThumbnailUrl, x => x.ConvertUsing<ImageFileResolver, ImageFile?>(y => y.Thumbnail));

            CreateMap<UserChannel, DetailedUserChannelDto>()
                .ForMember(x => x.BannerUrl, x => x.ConvertUsing<ImageFileResolver, ImageFile?>(y => y.Banner));

            CreateMap<UserChannel, UserChannelInfoDto>()
                .ForMember(x => x.BannerUrl, x => x.ConvertUsing<ImageFileResolver, ImageFile?>(y => y.Banner));

            CreateMap<UserChannel, UserChannelDto>();
        }

    }
}
