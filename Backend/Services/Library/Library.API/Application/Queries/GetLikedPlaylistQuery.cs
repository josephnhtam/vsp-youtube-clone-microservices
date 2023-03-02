using Application.Contracts;
using Library.API.Application.DtoModels;
using Library.Domain.Specifications;

namespace Library.API.Application.Queries {
    public class GetLikedPlaylistQuery : IQuery<PlaylistDto> {
        public string UserId { get; set; }
        public Pagination? Pagination { get; set; }
        public IndexPagination? IndexPagination { get; set; }

        public GetLikedPlaylistQuery (string userId, Pagination? pagination) {
            UserId = userId;
            Pagination = pagination;
        }

        public GetLikedPlaylistQuery (string userId, IndexPagination indexPagination) {
            UserId = userId;
            IndexPagination = indexPagination;
        }
    }
}
