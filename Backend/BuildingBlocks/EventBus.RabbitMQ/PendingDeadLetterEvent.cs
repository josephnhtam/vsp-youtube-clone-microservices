using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace EventBus.RabbitMQ {
    public class PendingDeadLetterEvent : PendingEventBase {

        public string DeadLetterQueue { get; private set; }
        public ReadOnlyMemory<byte> Body { get; private set; }
        public Exception Failure { get; private set; }

        public PendingDeadLetterEvent (string deadLetterQueue, ReadOnlyMemory<byte> body, Exception failure) {
            DeadLetterQueue = deadLetterQueue;
            Body = body;
            Failure = failure;
        }

        public override void Publish (IModel channel, IRabbitMQTopology topology, ILogger logger, Action<ulong>? onObtainSeqNo) {
            var basicProperties = channel.CreateBasicProperties();
            basicProperties.Persistent = true;
            basicProperties.Headers = new Dictionary<string, object>{
                { "Failure.Message", Failure.Message },
                { "Failure.StackTrace", Failure.StackTrace ?? string.Empty }
            };

            var seqNo = channel.NextPublishSeqNo;
            onObtainSeqNo?.Invoke(seqNo);

            channel.BasicPublish(exchange: string.Empty,
                                 routingKey: DeadLetterQueue,
                                 basicProperties: basicProperties,
                                 body: Body);
        }

    }
}
