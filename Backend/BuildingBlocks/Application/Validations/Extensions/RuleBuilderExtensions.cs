using Domain.Rules;
using FluentValidation;

namespace Application.Validations.Extensions {
    public static class RuleBuilderExtensions {
        public static IRuleBuilderOptions<T, TProperty> AdhereRule<T, TProperty, TRule>
            (this IRuleBuilder<T, TProperty> ruleBuilder, Func<TProperty, TRule> ruleFactory)
            where TRule : IBusinessRule {
            return ruleBuilder.Must(
                (x, val) => {
                    TRule rule = ruleFactory.Invoke(val);
                    return !rule.IsBroken();
                }
            ).WithMessage(
                (x, val) => {
                    TRule rule = ruleFactory.Invoke(val);
                    return rule.BrokenReason;
                }
            );
        }

        public static IRuleBuilderOptions<T, TProperty> AdhereRules<T, TProperty>
            (this IRuleBuilder<T, TProperty> ruleBuilder, Func<TProperty, IEnumerable<IBusinessRule>> rulesFactory) {
            return ruleBuilder.Must(
                (x, val) => {
                    foreach (var rule in rulesFactory.Invoke(val)) {
                        if (rule.IsBroken()) {
                            return false;
                        }
                    }
                    return true;
                }
            );
        }
    }
}
