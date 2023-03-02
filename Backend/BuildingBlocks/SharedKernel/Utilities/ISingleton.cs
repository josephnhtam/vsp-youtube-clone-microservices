namespace SharedKernel.Utilities {
    public interface ISingleton<TType> where TType : new() {
        private static TType? _instance;
        private static object _syncObject = new object();

        public static TType Instance {
            get {
                if (_instance != null) {
                    return _instance;
                }

                lock (_syncObject) {
                    _instance = new TType();
                }

                return _instance;
            }
        }
    }
}
