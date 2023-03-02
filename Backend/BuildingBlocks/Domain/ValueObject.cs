using System.Reflection;

namespace Domain {
    public abstract class ValueObject : IEquatable<ValueObject?> {

        private IEnumerable<PropertyInfo>? _properties;
        private IEnumerable<FieldInfo>? _fields;

        public static bool operator == (ValueObject? obj1, ValueObject? obj2) {
            if (ReferenceEquals(obj1, null) ^ ReferenceEquals(obj2, null)) {
                return false;
            }

            return ReferenceEquals(obj1, obj2) || Equals(obj1, obj2);
        }

        public static bool operator != (ValueObject? obj1, ValueObject? obj2) {
            return !(obj1 == obj2);
        }

        public bool Equals (ValueObject? obj) {
            return Equals(obj as object);
        }

        public override bool Equals (object? obj) {
            if (obj == null || GetType() != obj.GetType()) {
                return false;
            }

            return GetProperties().All(p => PropertiesAreEqual(obj, p))
                    && GetFields().All(f => FieldsAreEqual(obj, f));
        }

        public override int GetHashCode () {
            return GetProperties().Select(x => x.GetValue(this, null))
                .Concat(GetFields().Select(x => x.GetValue(this)))
                .Select(x => x != null ? x.GetHashCode() : 0)
                .Aggregate((x, y) => x ^ y);
        }

        private bool PropertiesAreEqual (object obj, PropertyInfo p) {
            return object.Equals(p.GetValue(this, null), p.GetValue(obj, null));
        }

        private bool FieldsAreEqual (object obj, FieldInfo f) {
            return object.Equals(f.GetValue(this), f.GetValue(obj));
        }

        private IEnumerable<PropertyInfo> GetProperties () {
            if (_properties == null) {
                _properties = GetType()
                    .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                    .Where(p => p.GetCustomAttribute(typeof(IgnoreMemberAttribute)) == null)
                    .ToList();
            }

            return _properties;
        }

        private IEnumerable<FieldInfo> GetFields () {
            if (_fields == null) {
                _fields = GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                    .Where(p => p.GetCustomAttribute(typeof(IgnoreMemberAttribute)) == null)
                    .ToList();
            }

            return _fields;
        }
    }
}
