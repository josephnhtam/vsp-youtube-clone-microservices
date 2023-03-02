using Domain;

namespace VideoStore.Domain.Models {
    public class ProcessedVideo : ValueObject {

        public Guid VideoFileId { get; private set; }
        public string Label { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public long Size { get; private set; }
        public int LengthSeconds { get; private set; }
        public string Url { get; private set; }

        private ProcessedVideo (Guid videoFileId, string label, int width, int height, long size, int lengthSeconds, string url) {
            VideoFileId = videoFileId;
            Label = label;
            Width = width;
            Height = height;
            Size = size;
            LengthSeconds = lengthSeconds;
            Url = url;
        }

        public static ProcessedVideo Create (Guid videoFileId, string label, int width, int height, long size, int lengthSeconds, string url) {
            return new ProcessedVideo(videoFileId, label, width, height, size, lengthSeconds, url);
        }

    }
}
