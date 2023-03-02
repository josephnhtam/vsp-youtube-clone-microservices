using Domain;

namespace VideoProcessor.Domain.Models {
    public class VideoProcessingStep : ValueObject {

        public string Label { get; private set; }
        public int Height { get; private set; }
        public bool Complete { get; private set; }

        private VideoProcessingStep () { }

        private VideoProcessingStep (string label, int height) {
            Label = label;
            Height = height;
            Complete = false;
        }

        public static VideoProcessingStep Create (string label, int height) {
            return new VideoProcessingStep(label, height);
        }

        public void SetComplete () {
            Complete = true;
        }

    }
}
