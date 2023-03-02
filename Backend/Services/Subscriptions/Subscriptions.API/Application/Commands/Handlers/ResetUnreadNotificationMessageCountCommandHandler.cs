using Application.Handlers;
using MediatR;
using Subscriptions.Domain.Contracts;

namespace Subscriptions.API.Application.Commands.Handlers {
    public class ResetUnreadNotificationMessageCountCommandHandler : ICommandHandler<ResetUnreadNotificationMessageCountCommand> {

        private readonly INotificationDataAccess _notificationDataAccess;

        public ResetUnreadNotificationMessageCountCommandHandler (INotificationDataAccess notificationDataAccess) {
            _notificationDataAccess = notificationDataAccess;
        }

        public async Task<Unit> Handle (ResetUnreadNotificationMessageCountCommand request, CancellationToken cancellationToken) {
            await _notificationDataAccess.ResetUnreadMessageCountAsync(request.UserId);
            return Unit.Value;
        }
    }
}
