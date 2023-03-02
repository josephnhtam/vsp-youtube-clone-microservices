using AutoMapper;
using Library.API.Application.AutoMapperProfiles;
using Library.API.Application.DtoModels;
using Library.Domain.Models;
using Library.Infrastructure.Contracts;
using MongoDB.Driver;
using SharedKernel.Exceptions;
using System.Diagnostics;

namespace Library.API.Application.Queries.Handlers.Services {
    public class DtoResolver : IDtoResolver {

        private static readonly ActivitySource _activitySource = new ActivitySource("Library");

        private readonly ICachedVideoRepository _videoRepository;
        private readonly ICachedUserProfileRepository _userProfileRepository;
        private readonly IMapper _mapper;
        private readonly FileUrlResolver _fileUrlResolver;

        public DtoResolver (
            ICachedVideoRepository videoRepository,
            ICachedUserProfileRepository userProfileRepository,
            IMapper mapper,
            FileUrlResolver fileUrlResolver) {
            _videoRepository = videoRepository;
            _userProfileRepository = userProfileRepository;
            _mapper = mapper;
            _fileUrlResolver = fileUrlResolver;
        }

        public async Task<List<VideoDto>> ResolveVideoDtos (List<Video> videos, CancellationToken cancellationToken = default) {
            using var activity = _activitySource.StartActivity("ResolveVideoDtos");

            var userIds = videos.Select(x => x.CreatorId).Distinct();
            var userProfiles = await _userProfileRepository.GetUserProfilesAsync(userIds, cancellationToken);

            var creatorProfileDtos = _mapper
                .Map<List<CreatorProfileDto>>(userProfiles, options => options.Items["resolveUrl"] = true)
                .ToDictionary(x => x.Id, x => x);

            var videoDtos = new List<VideoDto>();
            foreach (var video in videos) {
                if (creatorProfileDtos.TryGetValue(video.CreatorId, out var creatorProfile)) {
                    var videoDto = _mapper.Map<VideoDto>(
                        video,
                        options => {
                            options.Items["creator"] = creatorProfile;
                            options.Items["resolveUrl"] = true;
                        });

                    videoDtos.Add(videoDto);
                }
            }

            return videoDtos;
        }

        public async Task<PlaylistDto> ResolvePlaylistDto<TPlaylist, TPlaylistItem> (TPlaylist playlist, string? userId, int? itemStartPosition, CancellationToken cancellationToken = default)
            where TPlaylist : PlaylistBase<TPlaylistItem>
            where TPlaylistItem : PlaylistItem {
            using var activity = _activitySource.StartActivity("ResolvePlaylistDto");

            var videoIds = playlist.Items.Select(x => x.VideoId).Distinct();
            var videos = await _videoRepository.GetVideosAsync(videoIds, cancellationToken);

            var userIds = videos.Select(x => x.CreatorId).Union(new[] { playlist.UserId });
            var creatorProfiles = await _userProfileRepository.GetUserProfilesAsync(userIds, cancellationToken);

            if (creatorProfiles.Count == 0) {
                throw new AppException("Creator not found", null, StatusCodes.Status404NotFound);
            }

            var creatorProfileDtos = _mapper
                .Map<List<CreatorProfileDto>>(creatorProfiles, options => options.Items["resolveUrl"] = true)
                .ToDictionary(x => x.Id, x => x);

            var playlistCreatorProfile = creatorProfiles.FirstOrDefault(x => x.Id == playlist.UserId);
            if (playlistCreatorProfile == null) {
                throw new AppException("Creator not found", null, StatusCodes.Status404NotFound);
            }

            var creatorProfileDto = _mapper.Map<CreatorProfileDto>(playlistCreatorProfile);

            var items = playlist.Items.Select((item, index) => {
                Guid videoId = item.VideoId;
                Video? video = videos.FirstOrDefault(v => v.Id == videoId);
                VideoDtoBase videoDto = ToVideoDto(userId, creatorProfileDtos, videoId, video);

                return new PlaylistItemDto {
                    Id = item.Id,
                    Video = videoDto,
                    Position = item is OrderedPlaylistItem o ? o.Position : itemStartPosition + index,
                    CreateDate = item.CreateDate
                };
            }).OrderBy(x => x.Position).ToList();

            var playlistDto = new PlaylistDto {
                Id = playlist.Id.ToString(),
                CreatorProfile = creatorProfileDto,
                ItemsCount = playlist.ItemsCount,
                Items = items,
                CreateDate = playlist.CreateDate,
                UpdateDate = playlist.UpdateDate
            };

            return playlistDto;
        }

        public async Task<PlaylistDto> ResolvePlaylistDtoFromVideos (GetVideosResult result, string playlistId, string? userId, int? itemStartPosition, CancellationToken cancellationToken = default) {
            using var activity = _activitySource.StartActivity("ResolvePlaylistDto");

            var videos = result.Videos;

            var userIds = videos.Select(x => x.CreatorId).Distinct();
            var creatorProfiles = await _userProfileRepository.GetUserProfilesAsync(userIds, cancellationToken);

            if (creatorProfiles.Count == 0) {
                throw new AppException("Creator not found", null, StatusCodes.Status404NotFound);
            }

            var creatorProfileDtos = _mapper
                .Map<List<CreatorProfileDto>>(creatorProfiles, options => options.Items["resolveUrl"] = true)
                .ToDictionary(x => x.Id, x => x);

            var creatorProfile = creatorProfiles.First();
            var creatorProfileDto = creatorProfileDtos[creatorProfile.Id];
            var updateDate = creatorProfile.LastVideoUpdateDate ?? DateTimeOffset.UtcNow;

            var items = videos.Select((video, index) => {
                VideoDtoBase videoDto = ToVideoDto(userId, creatorProfileDtos, video.Id, video);

                return new PlaylistItemDto {
                    Id = video.Id,
                    Video = videoDto,
                    Position = itemStartPosition + index,
                    CreateDate = video.CreateDate
                };
            }).OrderBy(x => x.Position).ToList();

            var playlistDto = new PlaylistDto {
                Id = playlistId,
                CreatorProfile = creatorProfileDto,
                ItemsCount = result.TotalCount,
                Items = items,
                Visibility = PlaylistVisibility.Unlisted,
                CreateDate = updateDate,
                UpdateDate = updateDate
            };

            return playlistDto;
        }

        public async Task<List<PlaylistInfoDto>> ResolvePlaylistInfoDtos (List<Playlist> playlists, string? userId, CancellationToken cancellationToken = default) {
            using var activity = _activitySource.StartActivity("ResolvePlaylistInfoDtos");

            var ordererdItemsDict = playlists
                .Where(x => x.Items.Count > 0)
                .ToDictionary(x => x.Id, x => x.Items.OrderBy(y => y.Position).ToList());

            // It is expected that for most of the playlist, the first item is its first available video
            var firstVideoIds = ordererdItemsDict.Select(x => x.Value.First().VideoId).Distinct();
            var getFirstVideosTask = _videoRepository.GetAvailableVideosAsync(firstVideoIds, userId, cancellationToken);

            var playlistCreatorIds = playlists.Select(x => x.UserId).Distinct();
            var getPlaylistCreatorProfilesTask = _userProfileRepository.GetUserProfilesAsync(playlistCreatorIds, cancellationToken);
            await Task.WhenAll(getFirstVideosTask, getPlaylistCreatorProfilesTask);

            var firstVideos = getFirstVideosTask.Result;
            var playlistCreatorProfiles = getPlaylistCreatorProfilesTask.Result;

            var firstAvailableVideos = new Dictionary<Guid, Video>();
            if (!ExtractFirstAvailableVideos(firstVideos, true)) {
                // In cases some playlist doesn't have the first item as its first available video,
                // fallback to find its first available video from its 2nd to n-th items

                var otherVideosIds = ordererdItemsDict
                    .Where(items => !firstAvailableVideos.ContainsKey(items.Key))
                    .SelectMany(items => items.Value.Skip(1))
                    .Select(item => item.VideoId)
                    .Distinct();

                var otherVideos = await _videoRepository.GetAvailableVideosAsync(otherVideosIds, userId, cancellationToken);
                ExtractFirstAvailableVideos(otherVideos, false);
            }

            var creatorProfileDtos = _mapper
                .Map<List<CreatorProfileDto>>(playlistCreatorProfiles, options => options.Items["resolveUrl"] = true)
                .ToDictionary(x => x.Id, x => x);

            var infos = playlists
                .Select(playlist => {
                    var creatorProfile = creatorProfileDtos.GetValueOrDefault(playlist.UserId);
                    if (creatorProfile == null) {
                        return null;
                    }

                    var firstAvailableVideo = firstAvailableVideos.GetValueOrDefault(playlist.Id);

                    return new PlaylistInfoDto {
                        Id = playlist.Id,
                        Title = playlist.Title,
                        Visibility = playlist.Visibility,
                        ThumbnailUrl = _fileUrlResolver.ResolveFileUrl(firstAvailableVideo?.ThumbnailUrl, true),
                        VideoId = firstAvailableVideo?.Id,
                        CreatorProfile = creatorProfile,
                        ItemsCount = playlist.ItemsCount,
                        CreateDate = playlist.CreateDate,
                        UpdateDate = playlist.UpdateDate
                    };
                })
                .Where(x => x != null)
                .ToList();

            return infos!;

            bool ExtractFirstAvailableVideos (List<Video> videos, bool firstVideoOnly) {
                var playlistsWithoutAvailableVideos = ordererdItemsDict.Keys.Where(x => !firstAvailableVideos.ContainsKey(x));

                foreach (var playlist in playlistsWithoutAvailableVideos) {
                    IEnumerable<OrderedPlaylistItem> orderedItems = ordererdItemsDict[playlist];

                    if (firstVideoOnly) {
                        orderedItems = orderedItems.Take(1);
                    }

                    Video? firstAvailableVideo = null;
                    foreach (var item in orderedItems) {
                        if ((firstAvailableVideo = videos.FirstOrDefault(x => x.Id == item.VideoId)) != null) {
                            break;
                        }
                    }

                    if (firstAvailableVideo != null) {
                        firstAvailableVideos[playlist] = firstAvailableVideo;
                    }
                }

                return firstAvailableVideos.Count == ordererdItemsDict.Count;
            }
        }

        public async Task<List<PlaylistDto>> ResolvePlaylistDtos (List<Playlist> playlists, string? userId, CancellationToken cancellationToken = default) {
            using var activity = _activitySource.StartActivity("ResolvePlaylistDtos");

            var videoIds = playlists.SelectMany(x => x.Items).Select(x => x.VideoId).Distinct();
            var videos = await _videoRepository.GetVideosAsync(videoIds, cancellationToken);

            var userIds = videos.Select(x => x.CreatorId).Union(playlists.Select(x => x.UserId));
            var creatorProfiles = await _userProfileRepository.GetUserProfilesAsync(userIds, cancellationToken);

            if (creatorProfiles.Count == 0) {
                return new List<PlaylistDto>();
            }

            var creatorProfileDtos = _mapper
                .Map<List<CreatorProfileDto>>(creatorProfiles, options => options.Items["resolveUrl"] = true)
                .ToDictionary(x => x.Id, x => x);

            List<PlaylistDto> result = new List<PlaylistDto>();

            foreach (var playlist in playlists) {
                var playlistCreatorProfile = creatorProfiles.FirstOrDefault(x => x.Id == playlist.UserId);

                if (playlistCreatorProfile == null) {
                    continue;
                }

                var items = playlist.Items.Select((item, index) => {
                    Guid videoId = item.VideoId;
                    Video? video = videos.FirstOrDefault(v => v.Id == videoId);
                    VideoDtoBase videoDto = ToVideoDto(userId, creatorProfileDtos, videoId, video);

                    return new PlaylistItemDto {
                        Id = item.Id,
                        Video = videoDto,
                        Position = item.Position,
                        CreateDate = item.CreateDate
                    };
                }).OrderBy(x => x.Position).ToList();

                var creatorProfileDto = _mapper.Map<CreatorProfileDto>(playlistCreatorProfile);

                var playlistDto = new PlaylistDto {
                    Id = playlist.Id.ToString(),
                    CreatorProfile = creatorProfileDto,
                    Title = playlist.Title,
                    Description = playlist.Description,
                    Visibility = playlist.Visibility,
                    ItemsCount = playlist.ItemsCount,
                    Items = items,
                    CreateDate = playlist.CreateDate,
                    UpdateDate = playlist.UpdateDate
                };

                result.Add(playlistDto);
            }

            return result;
        }

        private VideoDtoBase ToVideoDto (string? userId, Dictionary<string, CreatorProfileDto> creatorProfileDtos, Guid videoId, Video? video) {
            VideoDtoBase videoDto;
            if (video == null) {
                videoDto = new HiddenVideoDto(videoId, null);
            } else {
                if (video.Visibility == VideoVisibility.Private && video.CreatorId != userId) {
                    videoDto = new HiddenVideoDto(videoId, VideoVisibility.Private);
                } else {
                    var creatorProfile = creatorProfileDtos.GetValueOrDefault(video.CreatorId);
                    if (creatorProfile == null) {
                        videoDto = new HiddenVideoDto(videoId, null);
                    } else {
                        videoDto = _mapper.Map<VideoDto>(
                            video,
                            options => {
                                options.Items["creator"] = creatorProfile;
                                options.Items["resolveUrl"] = true;
                            });
                    }
                }
            }

            return videoDto;
        }
    }
}
