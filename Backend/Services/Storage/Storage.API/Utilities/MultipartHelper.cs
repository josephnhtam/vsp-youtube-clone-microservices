using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.WebUtilities;

namespace Storage.API.Utilities {
    public static class MultipartHelper {

        public static async Task<Dictionary<string, MultipartSection>> GetAllMultipartSections (this HttpRequest request) {
            var boundary = request.GetMultipartBoundary();
            if (string.IsNullOrEmpty(boundary)) {
                throw new ArgumentException("This is not a multipart form request");
            }

            Dictionary<string, MultipartSection> result = new Dictionary<string, MultipartSection>();

            MultipartReader reader = new MultipartReader(boundary, request.Body);

            MultipartSection? section;
            while ((section = await reader.ReadNextSectionAsync()) != null) {
                var headerName = section.GetContentDispositionHeader()?.Name.ToString();

                if (!string.IsNullOrEmpty(headerName)) {
                    result[headerName] = section;
                }
            }

            return result;
        }

        public static async Task<MultipartSection?> FindMultipartSection (this HttpRequest request, string header) {
            var boundary = request.GetMultipartBoundary();
            if (string.IsNullOrEmpty(boundary)) {
                throw new ArgumentException("This is not a multipart form request");
            }

            MultipartReader reader = new MultipartReader(boundary, request.Body);

            MultipartSection? section;
            while ((section = await reader.ReadNextSectionAsync()) != null) {
                var headerName = section.GetContentDispositionHeader()?.Name.ToString();

                if (headerName == header) {
                    return section;
                }
            }

            return null;
        }

    }
}
