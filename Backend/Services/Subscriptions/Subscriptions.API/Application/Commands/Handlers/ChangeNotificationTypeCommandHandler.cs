using Application.Handlers;
using Domain.Contracts;
using MediatR;
using SharedKernel.Exceptions;
using Subscriptions.Domain.Contracts;

namespace Subscriptions.API.Application.Commands.Handlers {
    public class ChangeNotificationTypeCommandHandler : ICommandHandler<ChangeNotificationTypeCommand> {

        private readonly ISubscriptionRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public ChangeNotificationTypeCommandHandler (
            ISubscriptionRepository repository,
            IUnitOfWork unitOfWork) {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle (ChangeNotificationTypeCommand request, CancellationToken cancellationToken) {
            var subscription = await _repository.GetSubscriptionAsync(
                request.UserId, request.SubscriptionTargetId, false, false, cancellationToken);

            if (subscription == null) {
                throw new AppException("Subscription not found", null, StatusCodes.Status404NotFound);
            }

            subscription.SetNotificationType(request.NotificationType);
            await _unitOfWork.CommitAsync(cancellationToken);

            return Unit.Value;
        }

    }
}
