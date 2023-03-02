
namespace Library.API.Application.DtoModels {
    public class MovePlaylistItemRequestDto {
        public string Playlist { get; set; }
        public Guid ItemId { get; set; }
        public int ToPosition { get; set; }
    }
}
