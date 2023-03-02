namespace Storage.Infrastructure.Scanners {
    public interface IAntiVirusScanner {
        Task<bool> ScanAndClean (string filePath);
    }
}
