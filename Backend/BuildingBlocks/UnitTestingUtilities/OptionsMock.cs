using Microsoft.Extensions.Options;

namespace UnitTestingUtilities {
    public class OptionsMock<T> : IOptions<T> where T : class {
        public T Value { get; private set; }

        public OptionsMock (T value) {
            Value = value;
        }
    }
}
