using Library.Domain.Models;

namespace Library.API.Application.DtoModels {
    public class SimplePlaylistDto {
        public Guid Id { get; set; }
        public string? Title { get; set; }
        public PlaylistVisibility? Visibility { get; set; }
        public int ItemsCount { get; set; }
        public DateTimeOffset CreateDate { get; set; }
        public DateTimeOffset UpdateDate { get; set; }
    }
}
