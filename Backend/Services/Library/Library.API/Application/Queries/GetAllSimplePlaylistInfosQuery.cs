using Application.Contracts;
using Library.API.Application.DtoModels;
using Library.Domain.Specifications;

namespace Library.API.Application.Queries {
    public class GetAllSimplePlaylistInfosQuery : IQuery<GetSimplePlaylistInfosResponseDto> {
        public string UserId { get; set; }
        public Pagination? Pagination { get; set; }

        public GetAllSimplePlaylistInfosQuery (string userId, Pagination? pagination) {
            UserId = userId;
            Pagination = pagination;
        }
    }
}
