using Domain.Rules;

namespace Domain.Exceptions {
    public class BusinessRuleValidationException : Exception {

        private BusinessRuleValidationException () { }

        private BusinessRuleValidationException (string message) : base(message) { }

        public static BusinessRuleValidationException Create<TRule> (TRule rule) where TRule : IBusinessRule {
            return new BusinessRuleValidationException(rule.BrokenReason);
        }

    }
}
