namespace SimpleFEM.Core.Validation.Result;

internal interface IResultValidationRule
{
    IEnumerable<ValidationError<ResultValidationErrorCode>> Validate(ResultValidationContext context);
}
