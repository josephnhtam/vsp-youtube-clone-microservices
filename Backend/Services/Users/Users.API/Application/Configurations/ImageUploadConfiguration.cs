namespace Users.API.Application.Configurations {
    public class ImageUploadConfiguration {
        public string SecretKey { get; set; }
        public string Issuer { get; set; }
        public float ExpireMinutes { get; set; }
        public int MaxThumbnailSize { get; set; }
        public int MaxBannberSize { get; set; }
    }
}
