using Application.Handlers;
using History.API.Application.DtoModels;
using History.Domain.Contracts;

namespace History.API.Application.Queries.Handlers {
    public class GetUserHistorySettingsQueryHandler : IQueryHandler<GetUserHistorySettingsQuery, UserHistorySettingsDto?> {

        private readonly IUserProfileRepository _repository;

        public GetUserHistorySettingsQueryHandler (IUserProfileRepository repository) {
            _repository = repository;
        }

        public async Task<UserHistorySettingsDto?> Handle (GetUserHistorySettingsQuery request, CancellationToken cancellationToken) {
            var userProfile = await _repository.GetUserProfileAsync(request.UserId, false, cancellationToken);

            if (userProfile == null) {
                return null;
            }

            return new UserHistorySettingsDto {
                RecordWatchHistory = userProfile.RecordWatchHistory
            };
        }
    }
}
