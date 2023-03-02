
namespace Library.API.Application.DtoModels {
    public class AddVideoToPlaylistRequestDto {
        public Guid VideoId { get; set; }
        public string Playlist { get; set; }
    }
}
