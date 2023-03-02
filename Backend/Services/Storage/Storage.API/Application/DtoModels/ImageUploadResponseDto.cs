namespace Storage.API.Application.DtoModels {
    public class ImageUploadResponseDto {
        public Guid FileId { get; set; }
        public string Category { get; set; }
        public string? ContentType { get; set; }
        public long FileSize { get; set; }
        public string Url { get; set; }
        public string Token { get; set; }
    }
}
