namespace VideoProcessor.Application.Configurations {
    public class VideoGeneratorConfiguration {
        public int FFmpegThreadsCount { get; set; } = 2;
        public HardwareAcceleration HardwareAcceleration { get; set; } = new HardwareAcceleration();
        public int Device { get; set; } = 0;
    }

    public class HardwareAcceleration {
        public bool Enable { get; set; } = false;
        public string HardwareAccelerator { get; set; } = "auto";
        public string Decoder { get; set; } = "h264_cuvid";
        public string Encoder { get; set; } = "h264_nvenc";
    }
}
