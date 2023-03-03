using Infrastructure;

namespace VideoManager.UnitTests.Mocks {
    public abstract class UnitOfWorkMock : IUnitOfWork {

        private bool _isInTransaction = false;

        public abstract Task CommitAsync (CancellationToken cancellationToken = default);

        public async Task ExecuteOptimisticTransactionAsync (Func<Task> task, Action<ITransactionOptions>? configureOptions = null, CancellationToken cancellationToken = default) {
            try {
                _isInTransaction = true;
                await task();
            } finally {
                _isInTransaction = false;
            }
        }

        public Task ExecuteOptimisticUpdateAsync (Func<Task> task) {
            return task();
        }

        public async Task ExecuteTransactionAsync (Func<Task> task, Action<ITransactionOptions>? configureOptions = null, CancellationToken cancellationToken = default) {
            try {
                _isInTransaction = true;
                await task();
            } finally {
                _isInTransaction = false;
            }
        }

        public bool IsInTransaction () {
            return _isInTransaction;
        }

        public abstract void ResetContext ();
    }
}
