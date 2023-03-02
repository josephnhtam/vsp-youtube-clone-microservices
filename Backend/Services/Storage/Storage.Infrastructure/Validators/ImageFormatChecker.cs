using SixLabors.ImageSharp;

namespace Storage.Infrastructure.Validators {
    public class ImageFormatChecker : IImageFormatChecker {
        public async Task<string?> GetContentTypeAsync (FileStream fileStream) {
            try {
                var (image, format) = await Image.LoadWithFormatAsync(fileStream);
                return format.DefaultMimeType;
            } catch (Exception) {
                return null;
            }
        }
    }
}
