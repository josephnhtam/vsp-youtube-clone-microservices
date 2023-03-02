using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace EventBus.Extensions {
    public interface IEventBusBuilder {

        IServiceCollection Services { get; }

        IEventBusBuilder AddIntegrationEvent (Type type);

        IEventBusBuilder AddIntegrationEventHandler (Type type);

        IEventBusBuilder AddIntegrationEvent<T> ()
            where T : IntegrationEventBase;

        IEventBusBuilder AddIntegrationEventHandler<T> ()
            where T : IntegrationEventHandlerBase;

        IEventBusBuilder AddIntegrationEvents (params Assembly[] assemblies);

        IEventBusBuilder AddIntegrationEventHandlers (params Assembly[] assemblies);

    }

    public class EventBusBuilder : IEventBusBuilder {

        public IServiceCollection Services { get; private set; }

        private readonly List<Type> _eventTypes;
        private readonly List<Type> _eventHandlerTypes;

        public EventBusBuilder (
            IServiceCollection services,
            List<Type> integrationEventTypes,
            List<Type> integrationEventHandlerTypes) {
            Services = services;
            _eventTypes = integrationEventTypes;
            _eventHandlerTypes = integrationEventHandlerTypes;
        }

        public IEventBusBuilder AddIntegrationEvent (Type type) {
            if (!_eventTypes.Contains(type)) {
                _eventTypes.Add(type);
            }

            return this;
        }

        public IEventBusBuilder AddIntegrationEventHandler (Type type) {
            if (!_eventHandlerTypes.Contains(type) && IsIntegrationEventHandler(type)) {
                _eventHandlerTypes.Add(type);
                Services.AddScoped(type);
            }

            return this;
        }

        public IEventBusBuilder AddIntegrationEvent<T> () where T : IntegrationEventBase {
            var type = typeof(T);
            return AddIntegrationEvent(type);
        }

        public IEventBusBuilder AddIntegrationEventHandler<T> ()
            where T : IntegrationEventHandlerBase {

            var handlerType = typeof(T);

            if (!_eventHandlerTypes.Contains(handlerType)) {
                _eventHandlerTypes.Add(handlerType);
                Services.AddScoped(handlerType);
            }

            return this;
        }

        public IEventBusBuilder AddIntegrationEvents (params Assembly[] assemblies) {
            foreach (var assembly in assemblies) {
                foreach (var type in assembly.GetExportedTypes()) {
                    if (type.IsClass && !type.IsAbstract && typeof(IntegrationEventBase).IsAssignableFrom(type)) {
                        AddIntegrationEvent(type);
                    }
                }
            }

            return this;
        }

        public IEventBusBuilder AddIntegrationEventHandlers (params Assembly[] assemblies) {
            foreach (var assembly in assemblies) {
                foreach (var type in assembly.GetExportedTypes()) {
                    if (type.IsClass && !type.IsAbstract) {
                        AddIntegrationEventHandler(type);
                    }
                }
            }

            return this;
        }

        private static bool IsIntegrationEventHandler (Type type) {
            Type handlerType = type;

            while (!handlerType.IsGenericType || handlerType.GetGenericTypeDefinition() != typeof(IntegrationEventHandler<,>)) {
                if (handlerType.BaseType != null) {
                    handlerType = handlerType.BaseType;
                } else {
                    return false;
                }
            }

            return true;
        }
    }
}
