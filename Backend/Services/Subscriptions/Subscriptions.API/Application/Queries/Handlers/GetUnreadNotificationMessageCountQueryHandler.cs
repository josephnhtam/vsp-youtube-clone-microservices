using Application.Handlers;
using Subscriptions.Domain.Contracts;

namespace Subscriptions.API.Application.Queries.Handlers {
    public class GetUnreadNotificationMessageCountQueryHandler : IQueryHandler<GetUnreadNotificationMessageCountQuery, int> {

        private readonly INotificationDataAccess _notificationDataAccess;

        public GetUnreadNotificationMessageCountQueryHandler (INotificationDataAccess notificationDataAccess) {
            _notificationDataAccess = notificationDataAccess;
        }

        public async Task<int> Handle (GetUnreadNotificationMessageCountQuery request, CancellationToken cancellationToken) {
            return await _notificationDataAccess.GetUnreadMessageCountAsync(request.UserId);
        }
    }
}
