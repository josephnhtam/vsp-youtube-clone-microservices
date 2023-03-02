namespace Infrastructure.Idempotency {
    public interface IIdempotencyContext {
        Task<bool> IsOperationIdStoredAsync (string operationId, CancellationToken cancellationToken = default);
        Task<bool> StoreOperationIdAsync (string operationId, CancellationToken cancellationToken = default);
    }
}
