
namespace Library.API.Application.DtoModels {
    public class RemoveItemFromPlaylistRequestDto {
        public Guid ItemId { get; set; }
        public string Playlist { get; set; }
    }
}
