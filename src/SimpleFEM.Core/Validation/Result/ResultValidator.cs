using SimpleFEM.Core.Domain;
using SimpleFEM.Core.Results;

namespace SimpleFEM.Core.Validation.Result;

internal sealed class ResultValidator(IEnumerable<IResultValidationRule> rules)
{
    internal ValidationResult<ResultValidationErrorCode> Validate(
        FemModel model, AnalysisResult result) =>
        new(rules.SelectMany(rule => rule.Validate(model, result)));
}
