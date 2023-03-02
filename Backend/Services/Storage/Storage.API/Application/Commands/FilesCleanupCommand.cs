using Application.Contracts;

namespace Storage.API.Application.Commands {
    public class FilesCleanupCommand : ICommand {
        public Guid GroupId { get; set; }
        public List<Guid>? ExcludedFileIds { get; set; }
        public List<string>? ExcludedCategories { get; set; }
        public List<string>? ExcludedContentTypes { get; set; }
        public TimeSpan? CleanupDelay { get; set; }

        public FilesCleanupCommand (Guid groupId, List<Guid>? excludedFileIds, List<string>? excludedCategories, List<string>? excludedContentTypes, TimeSpan? cleanupDelay) {
            GroupId = groupId;
            ExcludedFileIds = excludedFileIds;
            ExcludedCategories = excludedCategories;
            ExcludedContentTypes = excludedContentTypes;
            CleanupDelay = cleanupDelay;
        }
    }
}
