using System.Text;

namespace Storage.API.Application.Services {
    // https://stackoverflow.com/questions/210650/validate-image-from-file-in-c-sharp
    public class ImageHeaderChecker : IImageFormatChecker {

        private static List<Format> _formats { get; set; }
        private static int _headerBytesCount { get; set; }

        static ImageHeaderChecker () {
            _formats = new List<Format> {
                new Format("image/bmp", Encoding.ASCII.GetBytes("BM")),
                new Format("image/gif", Encoding.ASCII.GetBytes("GIF")),
                new Format("image/png", new byte[] { 137, 80, 78, 71 }),
                new Format("image/tiff", new byte[] { 73, 73, 42 }),
                new Format("image/tiff", new byte[] { 77, 77, 42 }),
                new Format("image/jpeg", new byte[] { 255, 216, 255, 224 }),
                new Format("image/jpeg", new byte[] { 255, 216, 255, 225 }),
            };

            _headerBytesCount = 0;
            foreach (var format in _formats) {
                int bytesCount = format.Header.Length;
                if (bytesCount > _headerBytesCount) {
                    _headerBytesCount = bytesCount;
                }
            }
        }

        public async Task<string?> GetContentTypeAsync (FileStream fileStream) {
            byte[] header = new byte[_headerBytesCount];
            await fileStream.ReadAsync(header, 0, header.Length);

            foreach (var format in _formats) {
                bool match = true;
                for (int i = 0; i < format.Header.Length; i++) {
                    if (format.Header[i] != header[i]) {
                        match = false;
                        break;
                    }
                }

                if (match) {
                    return format.ContentType;
                }
            }

            return null;
        }

        private class Format {
            public string ContentType { get; init; }
            public byte[] Header { get; init; }
            public Format (string contentType, byte[] header) {
                ContentType = contentType;
                Header = header;
            }
        }

    }
}
