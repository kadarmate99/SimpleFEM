using SimpleFEM.Core.Domain;
using SimpleFEM.Core.Results;

namespace SimpleFEM.Core.Validation.Result.Rules;

internal sealed class EquilibriumRule : IResultValidationRule
{
    public IEnumerable<ValidationError<ResultValidationErrorCode>> Validate(FemModel model, AnalysisResult result)
    {
        // TODO: Implement IResultValidationRule EquilibriumRule
        return [];
    }
}
