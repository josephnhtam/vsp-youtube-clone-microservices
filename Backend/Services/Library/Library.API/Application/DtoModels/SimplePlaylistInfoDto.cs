namespace Library.API.Application.DtoModels {
    public class SimplePlaylistInfoDto {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public DateTimeOffset UpdateDate { get; set; }
    }
}
