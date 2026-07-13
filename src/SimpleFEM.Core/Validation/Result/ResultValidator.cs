namespace SimpleFEM.Core.Validation.Result;

internal sealed class ResultValidator(IEnumerable<IResultValidationRule> rules)
{
    internal ValidationResult<ResultValidationErrorCode> Validate(
        ResultValidationContext context) =>
        new(rules.SelectMany(rule => rule.Validate(context)));
}
