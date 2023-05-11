namespace Domain.Contracts {
    public interface IUnitOfWork {
        Task ExecuteOptimisticUpdateAsync (Func<Task> task);
        Task ExecuteTransactionAsync (Func<Task> task, Action<ITransactionOptions>? configureOptions = null, CancellationToken cancellationToken = default);
        Task ExecuteOptimisticTransactionAsync (Func<Task> task, Action<ITransactionOptions>? configureOptions = null, CancellationToken cancellationToken = default);
        Task CommitAsync (CancellationToken cancellationToken = default);
        bool IsInTransaction ();
        void ResetContext ();
    }

    public interface ITransactionOptions { }
}
