namespace Storage.Infrastructure.LocalStorage {
    public class LocalStorageConfiguration {
        public string? RelativePath { get; set; }
        public string? AbsolutePath { get; set; }
        public string RequestPath { get; set; }
        public bool ServeStaticFiles { get; set; } = true;
    }
}
