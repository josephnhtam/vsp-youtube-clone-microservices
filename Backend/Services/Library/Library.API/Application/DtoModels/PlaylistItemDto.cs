namespace Library.API.Application.DtoModels {
    public class PlaylistItemDto {
        public Guid Id { get; set; }
        public object Video { get; set; }
        public int? Position { get; set; }
        public DateTimeOffset CreateDate { get; set; }
    }
}
