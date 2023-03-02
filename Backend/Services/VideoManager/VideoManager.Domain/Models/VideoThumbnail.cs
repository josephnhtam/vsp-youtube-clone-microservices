using Domain;

namespace VideoManager.Domain.Models {
    public class VideoThumbnail : ValueObject {

        public Guid ImageFileId { get; private set; }
        public string Label { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public string Url { get; private set; }

        private VideoThumbnail (Guid imageFileId, string label, int width, int height, string url) {
            ImageFileId = imageFileId;
            Label = label;
            Width = width;
            Height = height;
            Url = url;
        }

        public static VideoThumbnail Create (Guid imageFileId, string label, int width, int height, string url) {
            return new VideoThumbnail(imageFileId, label, width, height, url);
        }

    }
}
