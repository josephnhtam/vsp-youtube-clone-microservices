namespace SharedKernel.Exceptions {
    public static class ExceptionIdentifiers {

        private static Dictionary<string, List<IExceptionIdentifier>> _identifiers;

        static ExceptionIdentifiers () {
            Register(null);
        }

        public static void Register (Action<IExceptionIdentifiersBuilder>? configure) {
            var builder = new ExceptionIdentifiersBuilder();
            AddDefaultIdentifiers(builder);
            configure?.Invoke(builder);
            _identifiers = builder.Build();
        }

        private static void AddDefaultIdentifiers (ExceptionIdentifiersBuilder builder) {
            builder.AddIdentifier(ExceptionCategories.Transient,
                new DefaultTransientExceptionIdentifier());

            builder.AddIdentifier(ExceptionCategories.ConstraintViolation,
                new DefaultConstraintViolationExceptionIdentifier());

            builder.AddIdentifier(ExceptionCategories.UniqueViolation,
                new DefaultUniqueViolationExceptionIdentifier());
        }

        public static bool Identify (this Exception ex, string category, params object?[] entities) {
            return Identify(ex, _identifiers, category, entities);
        }

        private static bool Identify (Exception ex, Dictionary<string, List<IExceptionIdentifier>> identifiersDict, string category, params object?[] entities) {
            if (identifiersDict.TryGetValue(category, out var identifiers)) {
                for (int i = 0; i < identifiers.Count; i++) {
                    if (identifiers[i].Identify(ex, entities)) {
                        return true;
                    }
                }
            }

            if (ex.InnerException != null) {
                return Identify(ex.InnerException, identifiersDict, category, entities);
            }

            return false;
        }

    }

    public interface IExceptionIdentifiersBuilder {
        void AddIdentifier (string category, IExceptionIdentifier identifier);
        void Clear ();
    }

    internal class ExceptionIdentifiersBuilder : IExceptionIdentifiersBuilder {

        private Dictionary<string, List<IExceptionIdentifier>> _identifiers;

        public ExceptionIdentifiersBuilder () {
            _identifiers = new Dictionary<string, List<IExceptionIdentifier>>();
        }

        public void Clear () {
            _identifiers.Clear();
        }

        public void AddIdentifier (string category, IExceptionIdentifier identifier) {
            if (!_identifiers.TryGetValue(category, out var list)) {
                list = new List<IExceptionIdentifier>();
                _identifiers[category] = list;
            }

            list.Add(identifier);
        }

        public Dictionary<string, List<IExceptionIdentifier>> Build () {
            return _identifiers;
        }

    }
}
