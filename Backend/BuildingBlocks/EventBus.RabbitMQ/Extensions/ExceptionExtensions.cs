using EventBus.RabbitMQ.Exceptions;
using RabbitMQ.Client.Exceptions;
using System.Net.Sockets;

namespace EventBus.RabbitMQ.Extensions {
    public static class ExceptionExtensions {

        public static bool IsTransientRabbitMQException (this Exception exception) {
            return
                exception is BrokerUnreachableException ||
                exception is ConnectFailureException ||
                exception is OperationInterruptedException ||
                exception is SocketException ||
                exception is AlreadyClosedException ||
                exception is PublishChannelStoppedException ||
                exception is RabbitMQNackException;
        }
    }
}
