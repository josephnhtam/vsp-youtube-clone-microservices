namespace VideoProcessor.Application.Infrastructure {
    public interface ITempDirectoryRepository {
        Task<string> GetTempDirectoryAsync (Guid id);
        Task RemoveTempDirectoryAsync (Guid id);
        Task RemoveAllTempDirectoriesAsync ();
    }
}
