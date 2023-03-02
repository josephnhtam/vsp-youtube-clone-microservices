using Application.Contracts;

namespace Library.API.Application.Commands {
    public class RemoveVideoFromPlaylistCommand : ICommand {
        public string UserId { get; set; }
        public Guid PlaylistId { get; set; }
        public Guid VideoId { get; set; }

        public RemoveVideoFromPlaylistCommand (string userId, Guid playlistId, Guid videoId) {
            UserId = userId;
            PlaylistId = playlistId;
            VideoId = videoId;
        }
    }
}
