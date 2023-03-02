
using Application.Handlers;
using SharedKernel.Exceptions;
using Users.Domain.Contracts;
using Users.Domain.Models;

namespace Users.API.Application.Queries.Handlers {
    public class GetUserProfileQueryHandler : IQueryHandler<GetUserProfileQuery, UserProfile> {

        private readonly IUserProfileRepository _repository;

        public GetUserProfileQueryHandler (IUserProfileRepository repository) {
            _repository = repository;
        }

        public async Task<UserProfile> Handle (GetUserProfileQuery request, CancellationToken cancellationToken) {
            if (string.IsNullOrEmpty(request.UserId) && string.IsNullOrEmpty(request.Handle)) {
                throw new AppException(null, null, StatusCodes.Status400BadRequest);
            }

            var userProfile = await
                (!string.IsNullOrEmpty(request.UserId) ?
                _repository.GetUserProfileByIdAsync(request.UserId, false, cancellationToken) :
                _repository.GetUserProfileByHandleAsync(request.Handle!, false, cancellationToken));

            if (request.ThrowIfNotFound && userProfile == null) {
                throw new AppException("User profile not found", null, StatusCodes.Status404NotFound);
            }

            return userProfile!;
        }

    }
}
