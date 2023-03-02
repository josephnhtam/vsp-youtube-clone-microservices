using Application.Contracts;

namespace Library.API.Application.Queries {
    public class GetPublicVideosCountQuery : IQuery<int> {
        public string UserId { get; set; }

        public GetPublicVideosCountQuery (string userId) {
            UserId = userId;
        }
    }
}
