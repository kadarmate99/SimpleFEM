using SimpleFEM.Core.Domain;
using SimpleFEM.Core.Results;

namespace SimpleFEM.Core.Validation.Result;

internal interface IResultValidationRule
{
    IEnumerable<ValidationError<ResultValidationErrorCode>> Validate(FemModel model, AnalysisResult result);
}
