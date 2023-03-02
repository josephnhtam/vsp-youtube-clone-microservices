namespace Storage.API.Application.Configurations {
    public class ImageStorageConfiguration {
        public string SecretKey { get; set; }
        public string Issuer { get; set; }
        public int ExpireMinutes { get; set; } = 60;
        public int BufferSize { get; set; } = 2048;
    }
}
