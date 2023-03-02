using EventBus.RabbitMQ.Extensions;

namespace EventBus.RabbitMQ {
    public class RabbitMQEventBusConfiguration {

        public RabbitMQPublishingConfiguration Publishing { get; private set; }
        public RabbitMQSubscribingConfiguration Subscribing { get; private set; }
        public RabbitMQQosConfiguration Qos { get; private set; }
        public IReadOnlyList<IRabbitMQRequeuePolicy> RequeuePolicies { get; private set; }

        public RabbitMQEventBusConfiguration (RabbitMQEventBusConfigurator configurator) {
            Publishing = configurator.Publishing;
            Subscribing = configurator.Subscribing;
            Qos = configurator.Qos;
            RequeuePolicies = configurator.RequeuePolicies;
        }

    }
}
