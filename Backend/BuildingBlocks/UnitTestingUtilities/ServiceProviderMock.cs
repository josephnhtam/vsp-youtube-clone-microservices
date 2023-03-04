using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace UnitTestingUtilities {
    public class ServiceProviderMock : IServiceProvider {

        internal ServiceScopeMock Scope { get; private set; }
        internal ServiceScopeMock RootScope { get; private set; }

        private Dictionary<Type, Func<object>> _serviceFactories = new Dictionary<Type, Func<object>>();

        public ServiceProviderMock (Action<IServiceProviderMockConfigurator>? configure = null, ServiceScopeMock? scope = null, ServiceScopeMock? rootScope = null) {
            Scope = scope ?? new ServiceScopeMock(this);
            RootScope = rootScope ?? new ServiceScopeMock(this);

            AddService<IServiceScopeFactory>(() => new ServiceScopeFactoryMock(configure, RootScope));

            var configurator = new ServiceProviderMockConfigurator(this);
            configure?.Invoke(configurator);
        }

        internal void AddService<T> (Func<T> serviceFactory) where T : class {
            _serviceFactories[typeof(T)] = serviceFactory;
        }

        public object? GetService (Type serviceType) {
            return _serviceFactories[serviceType]?.Invoke();
        }
    }

    public class ServiceScopeMock : IServiceScope {
        internal Dictionary<Type, object> Services = new Dictionary<Type, object>();

        public IServiceProvider ServiceProvider { get; private set; }

        public ServiceScopeMock (IServiceProvider serviceProvider) {
            ServiceProvider = serviceProvider;
        }

        public ServiceScopeMock (Action<IServiceProviderMockConfigurator>? configure, ServiceScopeMock? rootScope = null) {
            ServiceProvider = new ServiceProviderMock(configure, this, rootScope ?? this);
        }

        public void Dispose () {
            Services.Clear();
        }
    }

    public class ServiceScopeFactoryMock : IServiceScopeFactory {

        private readonly Action<IServiceProviderMockConfigurator>? _configure;
        private readonly ServiceScopeMock? _rootScope;

        public ServiceScopeFactoryMock (Action<IServiceProviderMockConfigurator>? configure = null, ServiceScopeMock? rootScope = null) {
            _configure = configure;
            _rootScope = rootScope;
        }

        public IServiceScope CreateScope () {
            return new ServiceScopeMock(_configure, _rootScope);
        }
    }

    public class ServiceProviderMockConfigurator : IServiceProviderMockConfigurator {

        private readonly ServiceProviderMock _serviceProvider;

        public ServiceProviderMockConfigurator (ServiceProviderMock serviceProvider) {
            _serviceProvider = serviceProvider;
        }

        public void AddScoped<T> (Func<T> serviceFactory) where T : class {
            var scopedServices = _serviceProvider.Scope.Services;
            scopedServices[typeof(T)] = serviceFactory.Invoke()!;
            _serviceProvider.AddService<T>(() => (scopedServices[typeof(T)] as T)!);
        }

        public void AddSingleton<T> (Func<T> serviceFactory) where T : class {
            var singletonServices = _serviceProvider.RootScope.Services;
            singletonServices[typeof(T)] = serviceFactory.Invoke()!;
            _serviceProvider.AddService<T>(() => (singletonServices[typeof(T)] as T)!);
        }

        public void AddTransient<T> (Func<T> serviceFactory) where T : class {
            _serviceProvider.AddService<T>(() => serviceFactory.Invoke()!);
        }

        public void Configure<T> (T configuration) where T : class {
            AddSingleton<IOptions<T>>(() => {
                return new OptionsMock<T>(configuration);
            });
        }
    }

    public interface IServiceProviderMockConfigurator {
        void AddScoped<T> (Func<T> serviceFactory) where T : class;
        void AddSingleton<T> (Func<T> serviceFactory) where T : class;
        void AddTransient<T> (Func<T> serviceFactory) where T : class;
        void Configure<T> (T configuration) where T : class;
    }
}
