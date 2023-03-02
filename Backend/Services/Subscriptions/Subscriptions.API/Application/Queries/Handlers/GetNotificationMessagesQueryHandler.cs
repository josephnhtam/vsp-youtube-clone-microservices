using Application.Handlers;
using AutoMapper;
using Microsoft.Extensions.Options;
using Subscriptions.API.Application.Configurations;
using Subscriptions.API.Application.DtoModels;
using Subscriptions.Domain.Contracts;

namespace Subscriptions.API.Application.Queries.Handlers {
    public class GetNotificationMessagesQueryHandler : IQueryHandler<GetNotificationMessagesQuery, GetNotificationMessageResponseDto> {

        private readonly INotificationDataAccess _notificationDataAccess;
        private readonly NotificationConfiguration _config;
        private readonly IMapper _mapper;

        public GetNotificationMessagesQueryHandler (
            INotificationDataAccess notificationDataAccess,
            IOptions<NotificationConfiguration> config,
            IMapper mapper) {
            _notificationDataAccess = notificationDataAccess;
            _config = config.Value;
            _mapper = mapper;
        }

        public async Task<GetNotificationMessageResponseDto> Handle (GetNotificationMessagesQuery request, CancellationToken cancellationToken) {
            var (messages, totalCount) = await _notificationDataAccess.GetMessagesAsync(
                request.UserId,
                request.Page,
                request.PageSize,
                TimeSpan.FromDays(_config.HistoryExpirationDays));

            return new GetNotificationMessageResponseDto {
                TotalCount = (int)totalCount,
                Messages = _mapper.Map<List<NotificationMessageDto>>(messages, options => options.Items["resolveUrl"] = true)
            };
        }

    }
}
