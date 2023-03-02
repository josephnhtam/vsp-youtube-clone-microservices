using Domain.Events;
using VideoManager.Domain.Models;

namespace VideoManager.Domain.DomainEvents {
    public class VideoUploadedDomainEvent : IDomainEvent {

        public Video Video { get; set; }

        public VideoUploadedDomainEvent (Video video) {
            Video = video;
        }

    }
}
