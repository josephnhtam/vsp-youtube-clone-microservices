using Domain;

namespace Users.Domain.Models {
    public class ImageFile : ValueObject {

        public Guid ImageFileId { get; private set; }
        public string Url { get; private set; }

        private ImageFile (Guid imageFileId, string url) {
            ImageFileId = imageFileId;
            Url = url;
        }

        public static ImageFile Create (Guid imageFileId, string url) {
            return new ImageFile(imageFileId, url);
        }

    }
}
