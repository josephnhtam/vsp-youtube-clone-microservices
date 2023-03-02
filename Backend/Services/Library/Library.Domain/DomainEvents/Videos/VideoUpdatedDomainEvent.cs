using Domain.Events;
using Library.Domain.Models;

namespace Library.Domain.DomainEvents.Videos {
    public class VideoUpdatedDomainEvent : IDomainEvent {

        public Guid VideoId { get; set; }
        public string CreatorId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Tags { get; set; }
        public string? ThumbnailUrl { get; set; }
        public string? PreviewThumbnailUrl { get; set; }
        public int? LengthSeconds { get; set; }
        public VideoVisibility Visibility { get; set; }
        public VideoStatus Status { get; set; }
        public DateTimeOffset? PublishDate { get; set; }
        public long Version { get; set; }

        public VideoUpdatedDomainEvent (Guid videoId, string creatorId, string title, string description, string tags, string? thumbnailUrl, string? prewviewThumbailUrl, int? lengthSeconds, VideoVisibility visibility, VideoStatus status, DateTimeOffset? publishDate, long version) {
            VideoId = videoId;
            CreatorId = creatorId;
            Title = title;
            Description = description;
            Tags = tags;
            ThumbnailUrl = thumbnailUrl;
            PreviewThumbnailUrl = prewviewThumbailUrl;
            LengthSeconds = lengthSeconds;
            Visibility = visibility;
            Status = status;
            PublishDate = publishDate;
            Version = version;
        }

    }
}
