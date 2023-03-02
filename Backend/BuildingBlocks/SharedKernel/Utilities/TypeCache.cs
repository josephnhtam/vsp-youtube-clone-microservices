using System.Collections.Concurrent;
using System.Reflection;

namespace SharedKernel.Utilities {
    public static class TypeCache {

        private static ConcurrentDictionary<string, Type?> _cachedTypes = new ConcurrentDictionary<string, Type?>();
        private static Dictionary<string, Assembly?> _assembliesByName;
        private static List<Assembly> _assembliesByIndex;

        static TypeCache () {
            _assembliesByName = new Dictionary<string, Assembly?>();
            _assembliesByIndex = new List<Assembly>();

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies()) {
                if (assembly.FullName != null) {
                    _assembliesByName[assembly.FullName] = assembly;
                    _assembliesByIndex.Add(assembly);
                }
            }

            _cachedTypes = new ConcurrentDictionary<string, Type?>();
        }

        private static bool TryDirectTypeLookup (string? assemblyName, string typeName, out Type? type) {
            if (assemblyName != null) {
                if (_assembliesByName.TryGetValue(assemblyName, out var assembly) && assembly != null) {
                    type = assembly.GetType(typeName, false);
                    return type != null;
                }
            }

            type = null;
            return false;
        }

        private static bool TryIndirectTypeLookup (string typeName, out Type? type) {
            for (int i = 0; i < _assembliesByIndex.Count; i++) {
                var assembly = _assembliesByIndex[i];
                type = assembly.GetType(typeName);
                if (type != null) {
                    return true;
                }
            }

            for (int i = 0; i < _assembliesByIndex.Count; i++) {
                var assembly = _assembliesByIndex[i];

                foreach (var foundType in assembly.GetTypes()) {
                    if (foundType.Name == typeName) {
                        type = foundType;
                        return true;
                    }
                }
            }

            type = null;
            return false;
        }

        public static void Reset () {
            _cachedTypes = new ConcurrentDictionary<string, Type?>();
        }

        public static Type? GetType (string name) {
            return GetType(name, null);
        }

        public static Type? GetType (string name, string? assemblyHint) {
            if (string.IsNullOrEmpty(name)) {
                return null;
            }

            Type? type;
            if (_cachedTypes.TryGetValue(name, out type) == false) {
                if (!TryDirectTypeLookup(assemblyHint, name, out type)) {
                    TryIndirectTypeLookup(name, out type);
                }

                _cachedTypes[name] = type;
            }

            return type;
        }
    }
}