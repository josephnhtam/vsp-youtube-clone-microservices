namespace Domain.Rules {
    public class CharactersRule : IBusinessRule {

        private readonly string? _text;
        private readonly string _propertyName;
        private readonly char[] _invalidCharacters;

        public CharactersRule (string? text, string propertyName, char[] invalidCharacters) {
            _text = text;
            _propertyName = propertyName;
            _invalidCharacters = invalidCharacters;
        }

        public virtual string BrokenReason {
            get {
                return $"{_propertyName} contains invalid character";
            }
        }

        public virtual bool IsBroken () {
            if (!string.IsNullOrEmpty(_text)) {
                if (_invalidCharacters.Any(c => _text.Contains(c))) {
                    return true;
                }
            }

            return false;
        }
    }
}
