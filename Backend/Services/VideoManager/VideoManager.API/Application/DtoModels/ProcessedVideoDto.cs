namespace VideoManager.API.Application.DtoModels {
    public class ProcessedVideoDto {
        public Guid VideoFileId { get; set; }
        public string Label { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public long Size { get; set; }
        public int LengthSeconds { get; set; }
        public string Url { get; set; }
    }
}
