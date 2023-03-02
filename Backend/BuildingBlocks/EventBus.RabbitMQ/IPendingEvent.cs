using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace EventBus.RabbitMQ {
    public interface IPendingEvent {
        void Publish (IModel channel, IRabbitMQTopology topology, ILogger logger, Action<ulong>? onObtainSeqNo);
        Task WaitAsync ();
        void SetComplete ();
        void SetCanceled ();
        void SetException (Exception exception);
    }
}
