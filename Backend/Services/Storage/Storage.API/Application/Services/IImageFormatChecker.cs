namespace Storage.API.Application.Services {
    public interface IImageFormatChecker {
        Task<string?> GetContentTypeAsync (FileStream fileStream);
    }
}
