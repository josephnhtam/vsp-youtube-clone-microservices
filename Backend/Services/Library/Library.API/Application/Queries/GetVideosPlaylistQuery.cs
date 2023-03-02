using Application.Contracts;
using Library.API.Application.DtoModels;
using Library.Domain.Specifications;

namespace Library.API.Application.Queries {
    public class GetVideosPlaylistQuery : IQuery<PlaylistDto> {
        public string? UserId { get; set; }
        public string VideosUserId { get; set; }
        public Pagination? Pagination { get; set; }
        public IndexPagination? IndexPagination { get; set; }

        public GetVideosPlaylistQuery (string? userId, string videosUserId, Pagination? pagination) {
            UserId = userId;
            VideosUserId = videosUserId;
            Pagination = pagination;
        }

        public GetVideosPlaylistQuery (string? userId, string videosUserId, IndexPagination indexPagination) {
            UserId = userId;
            VideosUserId = videosUserId;
            IndexPagination = indexPagination;
        }
    }
}
