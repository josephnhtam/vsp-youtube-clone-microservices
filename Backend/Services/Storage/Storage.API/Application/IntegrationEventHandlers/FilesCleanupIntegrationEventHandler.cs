using EventBus;
using MediatR;
using Storage.API.Application.Commands;
using Storage.Shared.IntegrationEvents;

namespace Storage.API.Application.IntegrationEventHandlers {
    public class FilesCleanupIntegrationEventHandler : IntegrationEventHandler<FilesCleanupIntegrationEvent, FilesCleanupIntegrationEventQueue> {

        private readonly IMediator _mediator;

        public FilesCleanupIntegrationEventHandler (IMediator mediator) {
            _mediator = mediator;
        }

        public override async Task Handle (FilesCleanupIntegrationEvent integrationEvent, IIncomingIntegrationEventProperties properties, IIncomingIntegrationEventContext context) {
            await _mediator.Send(new FilesCleanupCommand(
                integrationEvent.GroupId,
                integrationEvent.ExcludedFileIds,
                integrationEvent.ExcludedCategories,
                integrationEvent.ExcludedContentTypes,
                integrationEvent.CleanupDelay
            ));
        }

    }

    public class FilesCleanupIntegrationEventQueue : IntegrationEventQueue {
        public override void OnQueueCreating (IServiceProvider services, IIntegrationEventQueueProperties properties) {
            properties.QueueName = "Storage." + properties.QueueName;
        }
    }
}
