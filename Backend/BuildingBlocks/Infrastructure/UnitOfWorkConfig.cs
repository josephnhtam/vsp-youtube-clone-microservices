namespace Infrastructure {
    public class UnitOfWorkConfig {
        public int OptimisticConcurrencyConflictRetryCount { get; set; } = 10;
        public TimeSpan TransactionalEventAvailableDelay { get; set; } = TimeSpan.Zero;
    }
}
