using Application.Handlers;
using Users.Domain.Contracts;

namespace Users.API.Application.Queries.Handlers {
    public class SearchUserWatchHistoryQueryHandler : IQueryHandler<CheckForHandleAvailabilityQuery, bool> {

        private readonly IUserProfileRepository _userProfileRepository;

        public SearchUserWatchHistoryQueryHandler (IUserProfileRepository userProfileRepository) {
            _userProfileRepository = userProfileRepository;
        }

        public async Task<bool> Handle (CheckForHandleAvailabilityQuery request, CancellationToken cancellationToken) {
            return await _userProfileRepository.IsHandleAvailable(request.Handle, cancellationToken);
        }

    }
}
