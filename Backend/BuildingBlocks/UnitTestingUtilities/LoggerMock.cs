using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace UnitTestingUtilities {
    public class LoggerMock<T> : LoggerMock, ILogger<T> {
        public LoggerMock (ITestOutputHelper output, LogLevel minimumLevel) : base(output, minimumLevel) {
        }
    }

    public class LoggerMock : ILogger {

        private readonly ITestOutputHelper _output;
        private readonly LogLevel _minimumLevel;

        public LoggerMock (ITestOutputHelper output, LogLevel minimumLevel) {
            _output = output;
            _minimumLevel = minimumLevel;
        }

        public IDisposable? BeginScope<TState> (TState state) where TState : notnull {
            return null;
        }

        public bool IsEnabled (LogLevel logLevel) {
            return logLevel >= _minimumLevel;
        }

        public void Log<TState> (LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) {
            if (exception != null) {
                _output.WriteLine(formatter(state, exception));
                _output.WriteLine(exception.ToString());
            } else {
                _output.WriteLine(formatter(state, exception));
            }
        }
    }
}
