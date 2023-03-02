using Application.Contracts;
using Library.API.Application.DtoModels;
using Library.Domain.Specifications;

namespace Library.API.Application.Queries {
    public class GetAllPlaylistInfosQuery : IQuery<GetPlaylistInfosResponseDto> {
        public string UserId { get; set; }
        public Pagination? Pagination { get; set; }

        public GetAllPlaylistInfosQuery (string userId, Pagination? pagination) {
            UserId = userId;
            Pagination = pagination;
        }
    }
}
