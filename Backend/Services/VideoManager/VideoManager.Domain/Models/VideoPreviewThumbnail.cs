using Domain;

namespace VideoManager.Domain.Models {
    public class VideoPreviewThumbnail : ValueObject {

        public Guid ImageFileId { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public float LengthSeconds { get; private set; }
        public string Url { get; private set; }

        private VideoPreviewThumbnail (Guid imageFileId, int width, int height, float lengthSeconds, string url) {
            ImageFileId = imageFileId;
            Width = width;
            Height = height;
            LengthSeconds = lengthSeconds;
            Url = url;
        }

        public static VideoPreviewThumbnail Create (Guid imageFileId, int width, int height, float lengthSeconds, string url) {
            return new VideoPreviewThumbnail(imageFileId, width, height, lengthSeconds, url);
        }

    }
}
