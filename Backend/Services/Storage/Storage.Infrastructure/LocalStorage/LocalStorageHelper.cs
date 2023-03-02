using System.Reflection;

namespace Storage.Infrastructure.LocalStorage {
    public static class LocalStorageHelper {

        public static string GetRootPath (LocalStorageConfiguration config) {
            if (config.AbsolutePath != null) {
                return config.AbsolutePath;
            } else {
                string rootDir = Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location)!;
                return Path.Combine(rootDir, config.RelativePath!);
            }
        }

        public static string GetDirectory (string contentType) {
            foreach (var c in Path.GetInvalidFileNameChars()) {
                contentType = contentType.Replace(c, '-');
            }

            return contentType;
        }

    }
}
