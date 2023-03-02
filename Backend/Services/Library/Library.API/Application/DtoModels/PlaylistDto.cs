using Library.Domain.Models;

namespace Library.API.Application.DtoModels {
    public class PlaylistDto {
        public string Id { get; set; }
        public CreatorProfileDto CreatorProfile { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public PlaylistVisibility? Visibility { get; set; }
        public int ItemsCount { get; set; }
        public List<PlaylistItemDto> Items { get; set; }
        public DateTimeOffset CreateDate { get; set; }
        public DateTimeOffset UpdateDate { get; set; }
    }
}
