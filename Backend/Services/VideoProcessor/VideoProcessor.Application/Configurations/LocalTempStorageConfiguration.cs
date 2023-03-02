using System.Reflection;

namespace VideoProcessor.Application.Configurations {
    public class LocalTempStorageConfiguration {
        public string? RelativePath { get; set; }
        public string? AbsolutePath { get; set; }

        public string Path {
            get {
                if (AbsolutePath != null) {
                    return AbsolutePath;
                } else {
                    string rootDir = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location)!;
                    return System.IO.Path.Combine(rootDir, RelativePath!);
                }
            }
        }
    }
}
