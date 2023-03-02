namespace Users.API.Application.Configurations {
    public class ImageTokenValidationConfiguration {
        public string SecretKey { get; set; }
        public string[] Issuers { get; set; }
    }
}
