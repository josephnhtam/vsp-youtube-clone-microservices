namespace VideoProcessor.Application.Utilities {
    public class GracefulTerminationConfiguration {
        public int ShutdownTimeoutInMinutes { get; set; } = 30;
    }
}
