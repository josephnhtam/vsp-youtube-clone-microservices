using Application.Handlers;
using MediatR;
using Subscriptions.Domain.Contracts;

namespace Subscriptions.API.Application.Commands.Handlers {
    public class RemoveNotificationMessageFromUserCommandHandler : ICommandHandler<RemoveNotificationMessageFromUserCommand> {

        private readonly INotificationDataAccess _notificationDataAccess;

        public RemoveNotificationMessageFromUserCommandHandler (INotificationDataAccess notificationDataAccess) {
            _notificationDataAccess = notificationDataAccess;
        }

        public async Task<Unit> Handle (RemoveNotificationMessageFromUserCommand request, CancellationToken cancellationToken) {
            await _notificationDataAccess.RemoveMessageFromUserAsync(request.UserId, request.MessageId);
            return Unit.Value;
        }

    }
}
