using System.Reflection;

namespace VideoProcessor.Application.Utilities {
    public static class FFmpegHelper {

        public static string? FFmpegExecutablesPath { get; private set; } = null;

        public static string? FindFFmpegExecutablesPath () {
            var entryAssembly = Assembly.GetEntryAssembly();

            if (entryAssembly != null) {
                string path = Path.GetDirectoryName(entryAssembly.Location)!;
                if (FindFFmpegExecutables(path)) {
                    FFmpegExecutablesPath = path;
                    return path;
                }
            }

            var paths = Environment.GetEnvironmentVariable("PATH")!.Split(Path.PathSeparator);
            foreach (string path in paths) {
                if (FindFFmpegExecutables(path)) {
                    FFmpegExecutablesPath = path;
                    return path;
                }
            }

            return null;
        }

        private static bool FindFFmpegExecutables (string path) {
            if (Directory.Exists(path)) {
                IEnumerable<FileInfo> files = new DirectoryInfo(path).GetFiles();
                return HasExecutableFile(files, "ffmpeg") && HasExecutableFile(files, "ffprobe");
            }
            return false;
        }

        private static bool HasExecutableFile (IEnumerable<FileInfo> files, string fileName) {
            return files.FirstOrDefault((x) =>
                x.Name.Equals(fileName, StringComparison.InvariantCultureIgnoreCase) ||
                x.Name.Equals(fileName + ".exe", StringComparison.InvariantCultureIgnoreCase)) != null;
        }
    }
}
