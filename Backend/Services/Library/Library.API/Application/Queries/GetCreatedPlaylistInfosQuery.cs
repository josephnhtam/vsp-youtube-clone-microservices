using Application.Contracts;
using Library.API.Application.DtoModels;
using Library.Domain.Specifications;

namespace Library.API.Application.Queries {
    public class GetCreatedPlaylistInfosQuery : IQuery<GetPlaylistInfosResponseDto> {
        public string UserId { get; set; }
        public bool PublicOnly { get; set; }
        public Pagination? Pagination { get; set; }

        public GetCreatedPlaylistInfosQuery (string userId, bool publicOnly, Pagination? pagination) {
            UserId = userId;
            PublicOnly = publicOnly;
            Pagination = pagination;
        }
    }
}
