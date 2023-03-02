using Application.Contracts;
using Library.API.Application.DtoModels;
using Library.Domain.Specifications;

namespace Library.API.Application.Queries {
    public class GetVideosQuery : IQuery<GetVideosResponseDto> {
        public string UserId { get; set; }
        public bool PublicOnly { get; set; }
        public Pagination Pagination { get; set; }
        public VideoSort Sort { get; set; }

        public GetVideosQuery (string userId, bool publicOnly, Pagination pagination, VideoSort sort) {
            UserId = userId;
            PublicOnly = publicOnly;
            Pagination = pagination;
            Sort = sort;
        }
    }
}
