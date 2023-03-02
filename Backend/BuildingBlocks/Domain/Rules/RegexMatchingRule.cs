using System.Text.RegularExpressions;

namespace Domain.Rules {
    public class RegexMatchingRule : IBusinessRule {

        private readonly string? _text;
        private readonly string _propertyName;
        private readonly string _pattern;
        private readonly bool _allowEmpty;

        public RegexMatchingRule (string? text, string propertyName, string pattern, bool allowEmpty = true) {
            _text = text;
            _propertyName = propertyName;
            _pattern = pattern;
            _allowEmpty = allowEmpty;
        }

        public virtual string BrokenReason {
            get {
                return $"{_propertyName} is invalid";
            }
        }

        public virtual bool IsBroken () {
            if (_allowEmpty && string.IsNullOrEmpty(_text)) {
                return false;
            }

            string text = _text == null ? string.Empty : _text;
            return !Regex.IsMatch(text, _pattern);
        }
    }
}
