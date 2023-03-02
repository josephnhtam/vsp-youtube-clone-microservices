using Application.Contracts;
using Subscriptions.API.Application.DtoModels;

namespace Subscriptions.API.Application.Queries {
    public class GetNotificationMessagesQuery : IQuery<GetNotificationMessageResponseDto> {
        public string UserId { get; set; }
        public int? Page { get; set; }
        public int? PageSize { get; set; }

        public GetNotificationMessagesQuery (string userId, int? page, int? pageSize) {
            UserId = userId;
            Page = page;
            PageSize = pageSize;
        }
    }
}
