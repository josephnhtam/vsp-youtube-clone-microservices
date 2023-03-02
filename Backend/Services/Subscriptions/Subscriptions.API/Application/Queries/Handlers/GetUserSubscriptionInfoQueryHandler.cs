using Application.Handlers;
using SharedKernel.Exceptions;
using Subscriptions.API.Application.DtoModels;
using Subscriptions.Domain.Contracts;

namespace Subscriptions.API.Application.Queries.Handlers {
    public class GetUserSubscriptionInfoQueryHandler : IQueryHandler<GetUserSubscriptionInfoQuery, UserSubscriptionInfoDto> {

        private readonly IUserProfileRepository _repository;

        public GetUserSubscriptionInfoQueryHandler (IUserProfileRepository repository) {
            _repository = repository;
        }

        public async Task<UserSubscriptionInfoDto> Handle (GetUserSubscriptionInfoQuery request, CancellationToken cancellationToken) {
            var userProfile = await _repository.GetUserProfileAsync(request.UserId, cancellationToken);

            if (userProfile == null) {
                throw new AppException("User profile not found", null, StatusCodes.Status404NotFound);
            }

            return new UserSubscriptionInfoDto {
                SubscribersCount = userProfile.SubscribersCount,
                SubscriptionsCount = userProfile.SubscriptionsCount
            };
        }

    }
}
