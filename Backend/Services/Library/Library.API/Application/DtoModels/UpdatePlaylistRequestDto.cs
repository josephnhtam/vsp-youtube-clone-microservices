
using Library.Domain.Models;

namespace Library.API.Application.DtoModels {
    public class UpdatePlaylistRequestDto {
        public Guid PlaylistId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public PlaylistVisibility? Visibility { get; set; }
    }
}
