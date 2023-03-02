namespace Storage.Infrastructure.Validators {
    public interface IAntiVirusScanner {
        Task<bool> ScanAndClean (string filePath);
    }
}
