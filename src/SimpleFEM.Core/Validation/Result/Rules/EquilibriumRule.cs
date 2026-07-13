namespace SimpleFEM.Core.Validation.Result.Rules;

internal sealed class EquilibriumRule : IResultValidationRule
{
    public IEnumerable<ValidationError<ResultValidationErrorCode>> Validate(ResultValidationContext context)
    {
        // TODO: Implement IResultValidationRule EquilibriumRule
        return [];
    }
}
