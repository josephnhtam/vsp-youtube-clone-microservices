namespace Storage.API.Application.Configurations {
    public class VideoUploadTokenValidationConfiguration {
        public string SecretKey { get; set; }
        public string[] Issuers { get; set; }
    }
}
