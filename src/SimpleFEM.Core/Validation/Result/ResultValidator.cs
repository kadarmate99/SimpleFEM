using SimpleFEM.Core.Domain;
using SimpleFEM.Core.Results;

namespace SimpleFEM.Core.Validation.Result;

internal sealed class ResultValidator(IEnumerable<IResultValidationRule> rules)
{
    internal ValidationResult<ResultValidationErrorCode> Validate(FemModel model, AnalysisResult result)
    {
        var r = new ValidationResult<ResultValidationErrorCode>();
        foreach (var rule in rules)
            r.AddRange(rule.Validate(model, result));
        return r;
    }
}
