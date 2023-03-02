namespace Storage.API.Application.Configurations {
    public class ImageUploadTokenValidationConfiguration {
        public string SecretKey { get; set; }
        public string[] Issuers { get; set; }
    }
}
