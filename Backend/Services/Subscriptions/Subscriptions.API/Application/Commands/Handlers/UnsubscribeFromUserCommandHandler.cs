using Application.Handlers;
using Domain.Contracts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Exceptions;
using Subscriptions.Domain.Contracts;
using Subscriptions.Domain.Models;

namespace Subscriptions.API.Application.Commands.Handlers {
    public class UnsubscribeFromUserCommandHandler : ICommandHandler<UnsubscribeFromUserCommand> {

        private readonly ISubscriptionRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public UnsubscribeFromUserCommandHandler (
            ISubscriptionRepository repository,
            IUnitOfWork unitOfWork) {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle (UnsubscribeFromUserCommand request, CancellationToken cancellationToken) {
            if (request.SubscriptionTargetId == request.UserId) {
                throw new AppException("Subscribing your own self is not supported", null, StatusCodes.Status400BadRequest);
            }

            await _unitOfWork.ExecuteTransactionAsync(async () => {
                var dummy = Subscription.Create(request.UserId, request.SubscriptionTargetId, NotificationType.None, DateTimeOffset.UtcNow);

                await _repository.RemoveSubscriptionAsync(dummy, cancellationToken);

                try {
                    await _unitOfWork.CommitAsync(cancellationToken);
                } catch (DbUpdateException) {
                    throw new AppException("Failed to unsubscribe", null, StatusCodes.Status400BadRequest);
                }
            });

            return Unit.Value;
        }

    }
}
