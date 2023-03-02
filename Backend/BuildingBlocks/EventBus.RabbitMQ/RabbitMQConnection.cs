using Microsoft.Extensions.Logging;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System.Net.Sockets;

namespace EventBus.RabbitMQ {
    public class RabbitMQConnection : IDisposable, IRabbitMQConnection {

        private readonly Action<ConnectionFactory> _configureConnectionFactory;
        private readonly ILogger<RabbitMQConnection> _logger;

        private CancellationTokenSource _connectionAbortedSource;
        private IConnection? _connection;
        private bool _disposed;

        private object _lock;

        public bool IsConnected => _connection != null && _connection.IsOpen && !_disposed;
        public bool IsConnecting { get; private set; }

        public RabbitMQConnection (Action<ConnectionFactory> configureConnectionFactory, ILogger<RabbitMQConnection> logger) {
            _configureConnectionFactory = configureConnectionFactory;
            _logger = logger;
            _lock = new object();
        }

        public Task Connect (CancellationToken stoppingToken, out CancellationToken connectionAborted) {
            lock (_lock) {
                if (IsConnecting) throw new InvalidOperationException("Already connecting");
                IsConnecting = true;
            }

            if (_connection != null) {
                Stop();
            }

            _connectionAbortedSource = new CancellationTokenSource();
            connectionAborted = _connectionAbortedSource.Token;

            return Task.Run(() => {
                var retryPolicy = Policy
                .Handle<BrokerUnreachableException>()
                .Or<ConnectFailureException>()
                .Or<OperationInterruptedException>()
                .Or<SocketException>()
                .WaitAndRetryForever(
                    attempt => TimeSpan.FromSeconds(Math.Pow(2, Math.Min(5, attempt))),
                    (ex, time) => _logger.LogInformation(ex, "Retry to connect to RabbitMQ in {Seconds} seconds", time.Seconds));

                retryPolicy.Execute(() => {
                    if (stoppingToken.IsCancellationRequested) {
                        _connectionAbortedSource.Cancel();
                        return;
                    }

                    var connectionFactory = CreateConnectionFactory();

                    try {
                        _logger.LogInformation("Connecting to RabbitMQ ({HostName})", connectionFactory.Endpoint.HostName);
                        _connection = connectionFactory.CreateConnection();

                        if (IsConnected) {
                            _connection.ConnectionShutdown += OnConnectionShutdown;
                            _connection.CallbackException += OnCallbackException;
                            _connection.ConnectionBlocked += OnConnectionBlocked;

                            _logger.LogInformation("Connected to RabbitMQ ({HostName})", connectionFactory.Endpoint.HostName);

                            _connectionAbortedSource.TryReset();
                            IsConnecting = false;
                        } else {
                            _logger.LogError("Failed to connect to RabbitMQ ({HostName})", connectionFactory.Endpoint.HostName);
                            throw new BrokerUnreachableException(new Exception("Failed to connect to RabbitMQ"));
                        }
                    } catch (Exception ex) {
                        _logger.LogError(ex, "Failed to connect to RabbitMQ ({HostName})", connectionFactory.Endpoint.HostName);
                        throw;
                    }
                });
            });
        }

        public IConnection GetConnection () {
            if (IsConnecting || !IsConnected) throw new InvalidOperationException("Not yet connected to RabbitMQ");

            return _connection!;
        }

        private void OnCallbackException (object? sender, CallbackExceptionEventArgs e) {
            if (_disposed) return;

            _logger.LogWarning(e.Exception, "Callback exception is thrown");
            _connectionAbortedSource.Cancel();
        }

        private void OnConnectionBlocked (object? sender, ConnectionBlockedEventArgs e) {
            if (_disposed) return;

            _logger.LogWarning("Connection is blocked");
            _connectionAbortedSource.Cancel();
        }

        private void OnConnectionShutdown (object? sender, ShutdownEventArgs e) {
            if (_disposed) return;

            _logger.LogWarning("Connection lost");
            _connectionAbortedSource.Cancel();
        }

        private ConnectionFactory CreateConnectionFactory () {
            var connectionFactory = new ConnectionFactory();
            _configureConnectionFactory?.Invoke(connectionFactory);
            connectionFactory.DispatchConsumersAsync = true;
            connectionFactory.AutomaticRecoveryEnabled = false;
            return connectionFactory;
        }

        public void Stop () {
            if (_connection != null) {
                try {
                    if (_connection.IsOpen) {
                        _connection.Close();
                    }
                } catch (IOException) { }

                _connection.ConnectionShutdown -= OnConnectionShutdown;
                _connection.CallbackException -= OnCallbackException;
                _connection.ConnectionBlocked -= OnConnectionBlocked;

                _connection.Dispose();
                _connection = null;
            }
        }

        public void Dispose () {
            if (_disposed) return;

            _disposed = true;
            Stop();
        }

    }
}
