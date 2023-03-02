namespace VideoManager.API.Application.DtoModels {
    public class VideoThumbnailDto {
        public Guid ImageFileId { get; set; }
        public string Label { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string Url { get; set; }
    }
}
