using SharedKernel.Utilities;
using System.Text.Json;

namespace Domain.TransactionalEvents {
    public class TransactionalEvent {
        public string Category { get; private set; }
        public string Type { get; private set; }
        public string Data { get; private set; }

        private static JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions {
            IncludeFields = true
        };

        public TransactionalEvent (string category, string type, string data) {
            Category = category;
            Type = type;
            Data = data;
        }

        public TransactionalEvent (string category, object @event, Type? type = null) {
            Category = category;
            Type = type?.FullName! ?? @event.GetType().FullName!;
            Data = JsonSerializer.Serialize(@event);
        }

        public object? GetEvent () {
            if (string.IsNullOrEmpty(Type) || string.IsNullOrEmpty(Data)) {
                return null;
            }

            var type = TypeCache.GetType(Type);

            if (type == null) {
                return null;
            }

            return JsonSerializer.Deserialize(Data, type, JsonSerializerOptions);
        }
    }
}
