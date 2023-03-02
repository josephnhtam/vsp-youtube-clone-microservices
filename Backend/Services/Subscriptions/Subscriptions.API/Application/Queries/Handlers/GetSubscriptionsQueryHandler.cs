using Application.Handlers;
using AutoMapper;
using SharedKernel.Exceptions;
using Subscriptions.API.Application.DtoModels;
using Subscriptions.Domain.Contracts;

namespace Subscriptions.API.Application.Queries.Handlers {
    public class GetSubscriptionsQueryHandler : IQueryHandler<GetSubscriptionsQuery, SubscriptionsDto> {

        private readonly IUserProfileRepository _userProfileRepository;
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly IMapper _mapper;

        public GetSubscriptionsQueryHandler (IUserProfileRepository userProfileRepository, ISubscriptionRepository subscriptionRepository, IMapper mapper) {
            _userProfileRepository = userProfileRepository;
            _subscriptionRepository = subscriptionRepository;
            _mapper = mapper;
        }

        public async Task<SubscriptionsDto> Handle (GetSubscriptionsQuery request, CancellationToken cancellationToken) {
            if (request.IncludeTotalCount) {
                var userProfile = await _userProfileRepository.GetUserProfileAsync(request.UserId, cancellationToken);

                if (userProfile == null) {
                    throw new AppException("User profile not found", null, StatusCodes.Status404NotFound);
                }

                return new SubscriptionsDto {
                    TotalCount = userProfile.SubscriptionsCount,
                    Subscriptions = await GetSubscriptions(request, cancellationToken)
                };
            } else {
                return new SubscriptionsDto {
                    TotalCount = 0,
                    Subscriptions = await GetSubscriptions(request, cancellationToken)
                };
            }
        }

        private async Task<List<SubscriptionDto>> GetSubscriptions (GetSubscriptionsQuery request, CancellationToken cancellationToken) {
            var subscriptions = await _subscriptionRepository.GetSubscriptionsAsync(request.UserId, true, request.Sort, request.Pagination, cancellationToken);

            var subscriptionDtos = subscriptions.Select(x => {
                return new SubscriptionDto {
                    UserProfile = _mapper.Map<UserProfileDto>(
                            x.Target,
                            options => options.Items["resolveUrl"] = true),
                    SubscriptionDate = x.SubscriptionDate
                };
            }).ToList();
            return subscriptionDtos;
        }

    }
}
