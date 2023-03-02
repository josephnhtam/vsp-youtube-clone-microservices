using Domain.Events;
using VideoManager.Domain.Models;

namespace VideoManager.Domain.DomainEvents {
    public class ProcessedVideoAddedDomainEvent : IDomainEvent {

        public Video Video { get; set; }
        public ProcessedVideo ProcessedVideo { get; set; }

        public ProcessedVideoAddedDomainEvent (Video video, ProcessedVideo processedVideo) {
            Video = video;
            ProcessedVideo = processedVideo;
        }

    }
}
