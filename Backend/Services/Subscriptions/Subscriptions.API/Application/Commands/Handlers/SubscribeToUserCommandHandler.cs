using Application.Handlers;
using Domain.Contracts;
using MediatR;
using SharedKernel.Exceptions;
using Subscriptions.Domain.Contracts;
using Subscriptions.Domain.Models;

namespace Subscriptions.API.Application.Commands.Handlers {
    public class SubscribeToUserCommandHandler : ICommandHandler<SubscribeToUserCommand> {

        private readonly ISubscriptionRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public SubscribeToUserCommandHandler (
            ISubscriptionRepository repository,
            IUnitOfWork unitOfWork) {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle (SubscribeToUserCommand request, CancellationToken cancellationToken) {
            if (request.SubscriptionTargetId == request.UserId) {
                throw new AppException("Subscribing your own self is not supported", null, StatusCodes.Status400BadRequest);
            }

            await _unitOfWork.ExecuteTransactionAsync(async () => {
                try {
                    var subscription =
                        Subscription.Create(request.UserId, request.SubscriptionTargetId, request.NotificationType, DateTimeOffset.UtcNow);

                    await _repository.AddSubscriptionAsync(subscription);

                    await _unitOfWork.CommitAsync(cancellationToken);
                } catch (Exception ex) {
                    if (ex.Identify(ExceptionCategories.UniqueViolation)) {
                        throw new AppException("Already subscribed", null, StatusCodes.Status400BadRequest);
                    }

                    if (ex.Identify(ExceptionCategories.ConstraintViolation)) {
                        throw new AppException("User not found", null, StatusCodes.Status400BadRequest);
                    }
                }
            });

            return Unit.Value;
        }

    }
}
