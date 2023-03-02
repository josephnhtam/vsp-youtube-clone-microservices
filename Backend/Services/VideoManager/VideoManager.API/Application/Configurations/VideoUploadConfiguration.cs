namespace VideoManager.API.Application.Configurations {
    public class VideoUploadConfiguration {
        public string SecretKey { get; set; }
        public string Issuer { get; set; }
        public float ExpireMinutes { get; set; }
    }
}
