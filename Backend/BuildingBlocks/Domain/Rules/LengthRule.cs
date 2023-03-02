namespace Domain.Rules {
    public class LengthRule : IBusinessRule {

        private readonly string? _text;
        private readonly string _propertyName;
        private readonly int? _minLength;
        private readonly int? _maxLength;

        public LengthRule (string? text, string propertyName, int? minLength, int? maxLength) {
            _text = text;
            _propertyName = propertyName;
            _minLength = minLength;
            _maxLength = maxLength;
        }

        public virtual string BrokenReason {
            get {
                if (_minLength.HasValue && _maxLength.HasValue) {
                    if (_minLength.Value == 1) {
                        return $"{_propertyName} must be less than {_maxLength.Value} characters";
                    } else {
                        return $"{_propertyName} must be between {_minLength.Value} and {_maxLength.Value} in length";
                    }
                }

                if (_minLength.HasValue) {
                    if (_minLength.Value == 1) {
                        return $"{_propertyName} is required";
                    } else {
                        return $"{_propertyName} must be more than {_minLength.Value} characters";
                    }
                }

                if (_maxLength.HasValue) {
                    return $"{_propertyName} must be less than {_maxLength.Value} characters";
                }

                return string.Empty;
            }
        }

        public virtual bool IsBroken () {
            int length = string.IsNullOrEmpty(_text) ? 0 : _text.Length;

            if (_minLength.HasValue && _maxLength.HasValue) {
                return length < _minLength.Value || length > _maxLength.Value;
            }

            if (_minLength.HasValue) {
                return length < _minLength.Value;
            }

            if (_maxLength.HasValue) {
                return length > _maxLength.Value;
            }

            return false;
        }
    }
}
