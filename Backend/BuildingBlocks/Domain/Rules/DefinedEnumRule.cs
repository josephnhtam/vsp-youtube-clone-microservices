namespace Domain.Rules {
    public class DefinedEnumRule<TEnum> : IBusinessRule where TEnum : struct, Enum {

        private readonly TEnum? _enumValue;
        private readonly string _propertyName;
        private readonly bool _allowNull;

        public DefinedEnumRule (TEnum? enumValue, string propertyName, bool allowNull = false) {
            _enumValue = enumValue;
            _propertyName = propertyName;
            _allowNull = allowNull;
        }

        public string BrokenReason => $"{_propertyName} is invalid";

        public bool IsBroken () {
            if (!_allowNull && !_enumValue.HasValue) {
                return true;
            }

            return _enumValue.HasValue && !Enum.IsDefined(_enumValue.Value);
        }
    }
}
