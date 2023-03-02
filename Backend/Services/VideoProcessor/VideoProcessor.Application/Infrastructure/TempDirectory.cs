namespace VideoProcessor.Application.Infrastructure {
    public class TempDirectory {
        public TempDirectory (Guid id, string path) {
            Id = id;
            Path = path;
        }

        public Guid Id { get; private set; }
        public string Path { get; private set; }
    }
}
