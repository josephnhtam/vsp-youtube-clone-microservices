using Application.Contracts;

namespace Library.API.Application.Queries {
    public class GetPublicVideoTotalViewsQuery : IQuery<long> {
        public string UserId { get; set; }

        public GetPublicVideoTotalViewsQuery (string userId) {
            UserId = userId;
        }
    }
}
