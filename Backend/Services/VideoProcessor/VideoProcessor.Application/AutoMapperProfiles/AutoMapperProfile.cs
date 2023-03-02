using AutoMapper;
using VideoProcessor.Application.DtoModels;
using VideoProcessor.Domain.Models;

namespace VideoProcessor.Application.AutoMapperProfiles {
    public class AutoMapperProfile : Profile {

        public AutoMapperProfile () {
            CreateMap<ProcessedVideo, ProcessedVideoDto>();
            CreateMap<VideoPreviewThumbnail, VideoPreviewThumbnailDto>();
            CreateMap<VideoThumbnail, VideoThumbnailDto>();
        }

    }
}
