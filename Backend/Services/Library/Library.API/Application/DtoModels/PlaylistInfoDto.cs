using Library.Domain.Models;

namespace Library.API.Application.DtoModels {
    public class PlaylistInfoDto {
        public Guid Id { get; set; }
        public CreatorProfileDto CreatorProfile { get; set; }
        public string Title { get; set; }
        public string? ThumbnailUrl { get; set; }
        public Guid? VideoId { get; set; }
        public PlaylistVisibility? Visibility { get; set; }
        public int ItemsCount { get; set; }
        public DateTimeOffset CreateDate { get; set; }
        public DateTimeOffset UpdateDate { get; set; }
    }
}
