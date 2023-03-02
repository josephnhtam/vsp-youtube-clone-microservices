
namespace Library.API.Application.DtoModels {
    public class RemoveVideoFromPlaylistRequestDto {
        public Guid VideoId { get; set; }
        public string Playlist { get; set; }
    }
}
