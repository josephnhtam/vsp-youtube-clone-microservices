using Application.Handlers;
using AutoMapper;
using History.API.Application.DtoModels;
using History.Domain.Models;
using History.Infrastructure.Contracts;

namespace History.API.Application.Queries.Handlers {
    public class SearchUserWatchHistoryQueryHandler : IQueryHandler<SearchUserWatchHistoryQuery, SearchResponseDto> {

        private readonly IUserHistoryQueryManager _userHistoryQueryManager;
        private readonly ICachedVideoRepository _videoRepository;
        private readonly ICachedUserProfileRepository _userProfileRepository;
        private readonly IMapper _mapper;

        public SearchUserWatchHistoryQueryHandler (
            IUserHistoryQueryManager userHistoryQueryManager,
            ICachedVideoRepository videoRepository,
            ICachedUserProfileRepository userProfileRepository,
            IMapper mapper) {
            _userHistoryQueryManager = userHistoryQueryManager;
            _videoRepository = videoRepository;
            _userProfileRepository = userProfileRepository;
            _mapper = mapper;
        }

        public async Task<SearchResponseDto> Handle (SearchUserWatchHistoryQuery request, CancellationToken cancellationToken) {
            (long totalCount, List<(string id, Guid videoId, DateTimeOffset date)> results) =
                await _userHistoryQueryManager.SearchUserWatchHistory(request.SearchParams, cancellationToken);

            var videoIds = results.Select(x => x.videoId).Distinct();
            var videos = await _videoRepository.GetVideosAsync(videoIds, cancellationToken);

            var userIds = videos.Select(x => x.CreatorId).Distinct();
            var userProfiles = await _userProfileRepository.GetUserProfilesAsync(userIds, cancellationToken);

            var creatorProfileDtos = _mapper
                .Map<List<CreatorProfileDto>>(userProfiles, options => options.Items["resolveUrl"] = true)
                .ToDictionary(x => x.Id, x => x);

            var videoDtos = videos.Select(video => {
                if (video.Visibility == VideoVisibility.Private && video.CreatorId != request.SearchParams.UserId) {
                    return new HiddenVideoDto(video.Id, VideoVisibility.Private) as VideoDtoBase;
                } else {
                    return _mapper.Map<VideoDto>(video, options => {
                        options.Items["creator"] = creatorProfileDtos.GetValueOrDefault(video.CreatorId);
                        options.Items["resolveUrl"] = true;
                    }) as VideoDtoBase;
                }
            }).ToList();

            var items = results.Select(result => {
                return new UserWatchRecordDto {
                    Id = result.id,
                    Video = videoDtos.FirstOrDefault(v => v.Id == result.videoId)!,
                    Date = result.date
                };
            }).Where(x => x.Video != null).OfType<object>().ToList();

            return new SearchResponseDto {
                TotalCount = totalCount,
                Items = items
            };
        }

    }
}
