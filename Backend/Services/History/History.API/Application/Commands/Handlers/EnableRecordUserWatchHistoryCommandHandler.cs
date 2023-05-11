using Application.Handlers;
using Domain.Contracts;
using History.Domain.Contracts;
using MediatR;
using SharedKernel.Exceptions;

namespace History.API.Application.Commands.Handlers {
    public class EnableRecordUserWatchHistoryCommandHandler : ICommandHandler<EnableRecordUserWatchHistoryCommand> {

        private readonly IUserProfileRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public EnableRecordUserWatchHistoryCommandHandler (IUserProfileRepository repository, IUnitOfWork unitOfWork) {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle (EnableRecordUserWatchHistoryCommand request, CancellationToken cancellationToken) {
            var userProfile = await _repository.GetUserProfileAsync(request.UserId, false, cancellationToken);

            if (userProfile == null) {
                throw new AppException("User profile is not reigstered", null, StatusCodes.Status500InternalServerError);
            }

            userProfile.UpdateRecordWatchHistory(request.Enable);
            await _unitOfWork.CommitAsync(cancellationToken);

            return Unit.Value;
        }

    }
}
