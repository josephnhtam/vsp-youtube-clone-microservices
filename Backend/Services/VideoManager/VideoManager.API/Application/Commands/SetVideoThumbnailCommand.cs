using Application.Contracts;

namespace VideoManager.API.Application.Commands {
    public class SetVideoThumbnailCommand : ICommand {
        public Guid ThumbnailId { get; set; }

        public SetVideoThumbnailCommand (Guid thumbnailId) {
            ThumbnailId = thumbnailId;
        }
    }
}
