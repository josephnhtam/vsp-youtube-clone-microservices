namespace VideoManager.API.Application.DtoModels {
    public class VideoPreviewThumbnailDto {
        public Guid ImageFileId { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int LengthSeconds { get; set; }
        public string Url { get; set; }
    }
}
