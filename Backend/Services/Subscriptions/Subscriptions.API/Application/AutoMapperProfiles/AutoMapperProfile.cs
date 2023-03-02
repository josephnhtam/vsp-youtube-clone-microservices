using AutoMapper;
using Subscriptions.API.Application.DtoModels;
using Subscriptions.Domain.Models;

namespace Subscriptions.API.Application.AutoMapperProfiles {
    public class AutoMapperProfile : Profile {

        public AutoMapperProfile () {
            CreateMap<UserProfile, InternalUserProfileDto>().ReverseMap();

            CreateMap<UserProfile, UserProfileDto>()
                .ForMember(x => x.ThumbnailUrl, x => x.ConvertUsing<FileUrlResolver, string>());

            CreateMap<UserProfile, DetailedUserProfileDto>()
                .ForMember(x => x.ThumbnailUrl, x => x.ConvertUsing<FileUrlResolver, string>());

            CreateMap<NotificationMessageSender, NotificationMessageSenderDto>()
                .ForMember(x => x.ThumbnailUrl, x => x.ConvertUsing<FileUrlResolver, string>());

            CreateMap<NotificationMessageWithChecked, NotificationMessageDto>()
               .ForMember(x => x.ThumbnailUrl, x => x.ConvertUsing<FileUrlResolver, string>());
        }

    }
}
