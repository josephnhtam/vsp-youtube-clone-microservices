using Application.Handlers;
using SharedKernel.Exceptions;
using Subscriptions.API.Application.DtoModels;
using Subscriptions.Domain.Contracts;
using Subscriptions.Domain.Models;

namespace Subscriptions.API.Application.Queries.Handlers {
    public class GetSubscriptionStatusQueryHandler : IQueryHandler<GetSubscriptionStatusQuery, SubscriptionStatusDto?> {

        private readonly IUserProfileRepository _userProfileRepository;
        private readonly ISubscriptionRepository _subscriptionRepository;

        public GetSubscriptionStatusQueryHandler (IUserProfileRepository userProfileRepository, ISubscriptionRepository subscriptionRepository) {
            _userProfileRepository = userProfileRepository;
            _subscriptionRepository = subscriptionRepository;
        }

        public async Task<SubscriptionStatusDto?> Handle (GetSubscriptionStatusQuery request, CancellationToken cancellationToken) {
            if (string.IsNullOrEmpty(request.SubscriptionTargetId) &&
                string.IsNullOrEmpty(request.SubscriptionTargetHandle)) {
                throw new AppException(null, null, StatusCodes.Status400BadRequest);
            }

            var target = !string.IsNullOrEmpty(request.SubscriptionTargetId) ?
                await _userProfileRepository.GetUserProfileAsync(request.SubscriptionTargetId, cancellationToken) :
                await _userProfileRepository.GetUserProfileByHandleAsync(request.SubscriptionTargetHandle!, cancellationToken);

            if (target == null) return null;

            var subscribersCount = target.SubscribersCount;

            if (request.UserId == null) {
                return new SubscriptionStatusDto {
                    SubscribersCount = subscribersCount,
                    IsSubscribed = false,
                    NotificationType = NotificationType.None
                };
            } else {
                var subscription = await _subscriptionRepository
                    .GetSubscriptionAsync(request.UserId, target.Id, false, false, cancellationToken);

                return new SubscriptionStatusDto {
                    SubscribersCount = subscribersCount,
                    IsSubscribed = subscription != null,
                    NotificationType = subscription?.NotificationType ?? NotificationType.None
                };
            }
        }

    }
}
