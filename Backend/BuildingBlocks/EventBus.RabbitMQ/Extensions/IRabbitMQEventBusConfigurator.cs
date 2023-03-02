using Microsoft.Extensions.Diagnostics.HealthChecks;
using RabbitMQ.Client;

namespace EventBus.RabbitMQ.Extensions {
    public interface IRabbitMQEventBusConfigurator {
        IRabbitMQEventBusConfigurator ConfigureConnectionFactory (Action<ConnectionFactory> configure);
        IRabbitMQEventBusConfigurator ConfigurePublishing (Action<RabbitMQPublishingConfiguration> configure);
        IRabbitMQEventBusConfigurator ConfigureSubscribing (Action<RabbitMQSubscribingConfiguration> configure);
        IRabbitMQEventBusConfigurator ConfigureQos (Action<RabbitMQQosConfiguration> configure);
        IRabbitMQEventBusConfigurator AddRequeuePolicy (IRabbitMQRequeuePolicy requeuePolicy);
        IRabbitMQEventBusConfigurator ClearRequeuePolicy ();
        IRabbitMQEventBusConfigurator AddHealthCheck (string? name = null, HealthStatus? failureStatus = null, IEnumerable<string>? tags = null, TimeSpan? timeout = null);
    }
}
