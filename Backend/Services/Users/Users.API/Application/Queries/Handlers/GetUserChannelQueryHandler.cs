
using Application.Handlers;
using SharedKernel.Exceptions;
using Users.Domain.Contracts;
using Users.Domain.Models;

namespace Users.API.Application.Queries.Handlers {
    public class GetUserChannelQueryHandler : IQueryHandler<GetUserChannelQuery, UserChannel> {

        private readonly IUserChannelRepository _repository;

        public GetUserChannelQueryHandler (IUserChannelRepository repository) {
            _repository = repository;
        }

        public async Task<UserChannel> Handle (GetUserChannelQuery request, CancellationToken cancellationToken) {
            if (string.IsNullOrEmpty(request.UserId) && string.IsNullOrEmpty(request.Handle)) {
                throw new AppException(null, null, StatusCodes.Status400BadRequest);
            }

            var userChannel = await
                (!string.IsNullOrEmpty(request.UserId) ?
                _repository.GetUserChannelByIdAsync(
                    request.UserId,
                    request.MaxSectionItemsCount,
                    cancellationToken) :
                _repository.GetUserChannelByHandleAsync(
                    request.Handle!,
                    request.MaxSectionItemsCount,
                    cancellationToken));

            if (userChannel == null) {
                throw new AppException("User channel not found", null, StatusCodes.Status404NotFound);
            }

            return userChannel;
        }

    }
}
