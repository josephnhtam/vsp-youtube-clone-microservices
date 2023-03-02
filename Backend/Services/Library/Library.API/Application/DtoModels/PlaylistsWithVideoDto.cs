namespace Library.API.Application.DtoModels {
    public class PlaylistsWithVideoDto {
        public bool IsAddedToWatchLaterPlaylist { get; set; }
        public List<SimplePlaylistDto> PlaylistsWithVideo { get; set; }
        public List<SimplePlaylistDto> PlaylistsWithoutVideo { get; set; }
    }
}
