using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace EventBus.RabbitMQ {
    public abstract class PendingEventBase : IPendingEvent {

        private TaskCompletionSource _complete;

        public PendingEventBase () {
            _complete = new TaskCompletionSource();
        }

        public abstract void Publish (IModel channel, IRabbitMQTopology topology, ILogger logger, Action<ulong>? onObtainSeqNo);

        public void SetCanceled () {
            _complete.SetCanceled();
        }

        public void SetComplete () {
            _complete.SetResult();
        }

        public void SetException (Exception exception) {
            _complete.SetException(exception);
        }

        public Task WaitAsync () {
            return _complete.Task;
        }

    }
}
