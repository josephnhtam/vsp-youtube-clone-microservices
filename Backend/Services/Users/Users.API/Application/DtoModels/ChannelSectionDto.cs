namespace Users.API.Application.DtoModels {
    public class ChannelSectionDto {
        public ChannelSectionType Type { get; set; }
        public Guid? Id { get; set; }
        public SectionContent? Content { get; set; }
    }

    public class SectionContent {
        public string? Title { get; set; }
        public Guid? PlaylistId { get; set; }
        public List<Guid>? PlaylistIds { get; set; }
    }

    public enum ChannelSectionType {
        Videos = 0,
        CreatedPlaylists = 1,
        SinglePlaylist = 2,
        MultiplePLaylists = 4
    }
}
