using Domain;

namespace VideoProcessor.Domain.Models {
    public class VideoInfo : ValueObject {

        public int Width { get; private set; }
        public int Height { get; private set; }
        public long Size { get; private set; }
        public int LengthSeconds { get; private set; }

        private VideoInfo () {
        }

        private VideoInfo (int width, int height, long size, int lengthSeconds) {
            Width = width;
            Height = height;
            Size = size;
            LengthSeconds = lengthSeconds;
        }

        public static VideoInfo Create (int width, int height, long size, int lengthSeconds) {
            return new VideoInfo(width, height, size, lengthSeconds);
        }
    }
}
