using Application.Handlers;
using SharedKernel.Exceptions;
using Subscriptions.API.Application.DtoModels;
using Subscriptions.Infrastructure.Services;

namespace Subscriptions.API.Application.Queries.Handlers {
    public class GetSubscriptionTargetIdsQueryHandler : IQueryHandler<GetSubscriptionTargetIdsQuery, SubscriptionTargetIdsDto> {

        private readonly ISubscriptionQueryManager _queryManager;

        public GetSubscriptionTargetIdsQueryHandler (ISubscriptionQueryManager queryManager) {
            _queryManager = queryManager;
        }

        public async Task<SubscriptionTargetIdsDto> Handle (GetSubscriptionTargetIdsQuery request, CancellationToken cancellationToken) {
            var subscriptionTargetIds = await _queryManager.GetSubscriptionTargetIdsAsync(request.UserId, cancellationToken);

            if (subscriptionTargetIds == null) {
                throw new AppException("User not found", null, StatusCodes.Status404NotFound);
            }

            return new SubscriptionTargetIdsDto {
                SubscriptionTargetIds = subscriptionTargetIds
            };
        }

    }
}
