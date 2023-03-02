using Application.Handlers;
using MediatR;
using Subscriptions.Domain.Contracts;

namespace Subscriptions.API.Application.Commands.Handlers {
    public class MarkNotificationMessageCheckedCommandHandler : ICommandHandler<MarkNotificationMessageCheckedCommand> {

        private readonly INotificationDataAccess _notificationDataAccess;

        public MarkNotificationMessageCheckedCommandHandler (INotificationDataAccess notificationDataAccess) {
            _notificationDataAccess = notificationDataAccess;
        }

        public async Task<Unit> Handle (MarkNotificationMessageCheckedCommand request, CancellationToken cancellationToken) {
            await _notificationDataAccess.MarkMessageCheckedAsync(request.UserId, request.MessageId);
            return Unit.Value;
        }

    }
}
