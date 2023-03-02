using Microsoft.Extensions.Diagnostics.HealthChecks;
using RabbitMQ.Client;

namespace EventBus.RabbitMQ.Extensions {
    public class RabbitMQEventBusConfigurator : IRabbitMQEventBusConfigurator {

        public Action<ConnectionFactory> ConnectionFactory { get; private set; }
        public RabbitMQPublishingConfiguration Publishing { get; private set; }
        public RabbitMQSubscribingConfiguration Subscribing { get; private set; }
        public RabbitMQQosConfiguration Qos { get; private set; }
        public List<IRabbitMQRequeuePolicy> RequeuePolicies { get; private set; }
        public HealthCheckConfiguration? HealthCheckConfig { get; private set; }

        public RabbitMQEventBusConfigurator () {
            ConnectionFactory = (cf) => { };
            Publishing = new RabbitMQPublishingConfiguration();
            Subscribing = new RabbitMQSubscribingConfiguration();
            Qos = new RabbitMQQosConfiguration();
            RequeuePolicies = new List<IRabbitMQRequeuePolicy>() { new DefaultRabbitMQRequeuePolicy() };
            HealthCheckConfig = null;
        }

        public IRabbitMQEventBusConfigurator ConfigureConnectionFactory (Action<ConnectionFactory> configure) {
            ConnectionFactory = configure;
            return this;
        }

        public IRabbitMQEventBusConfigurator ConfigurePublishing (Action<RabbitMQPublishingConfiguration> configure) {
            configure.Invoke(Publishing);
            return this;
        }

        public IRabbitMQEventBusConfigurator ConfigureSubscribing (Action<RabbitMQSubscribingConfiguration> configure) {
            configure.Invoke(Subscribing);
            return this;
        }

        public IRabbitMQEventBusConfigurator ConfigureQos (Action<RabbitMQQosConfiguration> configure) {
            configure.Invoke(Qos);
            return this;
        }

        public IRabbitMQEventBusConfigurator AddRequeuePolicy (IRabbitMQRequeuePolicy requeuePolicy) {
            if (!RequeuePolicies.Contains(requeuePolicy)) {
                RequeuePolicies.Add(requeuePolicy);
            }
            return this;
        }

        public IRabbitMQEventBusConfigurator ClearRequeuePolicy () {
            RequeuePolicies.Clear();
            return this;
        }

        public IRabbitMQEventBusConfigurator AddHealthCheck (string? name = null, HealthStatus? failureStatus = null, IEnumerable<string>? tags = null, TimeSpan? timeout = null) {
            HealthCheckConfig = new HealthCheckConfiguration(name, failureStatus, tags, timeout);
            return this;
        }
    }

    public record HealthCheckConfiguration (string? Name = null, HealthStatus? FailureStatus = null, IEnumerable<string>? Tags = null, TimeSpan? Timeout = null);
}
