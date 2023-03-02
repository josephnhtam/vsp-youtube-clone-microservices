namespace Storage.Infrastructure.Validators {
    public interface IImageFormatChecker {
        Task<string?> GetContentTypeAsync (FileStream fileStream);
    }
}
