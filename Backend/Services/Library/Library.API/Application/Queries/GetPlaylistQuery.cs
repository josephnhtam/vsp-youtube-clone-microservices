using Application.Contracts;
using Library.API.Application.DtoModels;
using Library.Domain.Specifications;

namespace Library.API.Application.Queries {
    public class GetPlaylistQuery : IQuery<PlaylistDto> {
        public string? UserId { get; set; }
        public Guid PlaylistId { get; set; }
        public Pagination? Pagination { get; set; }
        public IndexPagination? IndexPagination { get; set; }

        public GetPlaylistQuery (string? userId, Guid playlistId, Pagination? pagination) {
            UserId = userId;
            PlaylistId = playlistId;
            Pagination = pagination;
        }

        public GetPlaylistQuery (string? userId, Guid playlistId, IndexPagination indexPagination) {
            UserId = userId;
            PlaylistId = playlistId;
            IndexPagination = indexPagination;
        }
    }
}
