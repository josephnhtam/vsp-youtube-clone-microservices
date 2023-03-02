using Application.Handlers;
using Library.Infrastructure.Contracts;

namespace Library.API.Application.Queries.Handlers {
    public class GetPublicVideoTotalViewsQueryHandler : IQueryHandler<GetPublicVideoTotalViewsQuery, long> {

        private readonly IVideoQueryHelper _queryHelper;

        public GetPublicVideoTotalViewsQueryHandler (IVideoQueryHelper queryHelper) {
            _queryHelper = queryHelper;
        }

        public async Task<long> Handle (GetPublicVideoTotalViewsQuery request, CancellationToken cancellationToken) {
            return await _queryHelper.GetVideoTotalViews(request.UserId, true, cancellationToken);
        }
    }
}
