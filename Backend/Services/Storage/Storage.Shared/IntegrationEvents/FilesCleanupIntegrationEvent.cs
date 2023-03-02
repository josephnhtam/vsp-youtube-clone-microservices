using EventBus;

namespace Storage.Shared.IntegrationEvents {
    public class FilesCleanupIntegrationEvent : IntegrationEvent<FilesCleanupIntegrationEventTopic> {
        public Guid GroupId { get; set; }
        public List<Guid>? ExcludedFileIds { get; set; }
        public List<string>? ExcludedCategories { get; set; }
        public List<string>? ExcludedContentTypes { get; set; }
        public TimeSpan? CleanupDelay { get; set; }

        public FilesCleanupIntegrationEvent (Guid groupId, List<Guid>? excludedFileIds = null, List<string>? excludedCategories = null, List<string>? excludedContentTypes = null, TimeSpan? cleanupDelay = null) {
            GroupId = groupId;
            ExcludedFileIds = excludedFileIds;
            ExcludedCategories = excludedCategories;
            ExcludedContentTypes = excludedContentTypes;
            CleanupDelay = cleanupDelay;
        }
    }

    public class FilesCleanupIntegrationEventTopic : IntegrationEventTopic {
        public override void OnTopicCreating (IServiceProvider services, IIntegrationEventTopicProperties properties) {
            properties.TopicName = "Storage.Shared." + properties.TopicName;
        }
    }
}
