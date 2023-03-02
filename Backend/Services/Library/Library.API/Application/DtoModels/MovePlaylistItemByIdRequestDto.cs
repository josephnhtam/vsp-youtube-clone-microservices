
namespace Library.API.Application.DtoModels {
    public class MovePlaylistItemByIdRequestDto {
        public string Playlist { get; set; }
        public Guid ItemId { get; set; }
        public Guid? PrecedingItemId { get; set; }
    }
}
