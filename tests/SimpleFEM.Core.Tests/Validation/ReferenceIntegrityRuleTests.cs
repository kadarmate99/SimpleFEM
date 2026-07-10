using SimpleFEM.Core.Domain.Supports;
using SimpleFEM.Core.Elements;
using SimpleFEM.Core.Loads;
using SimpleFEM.Core.Validation.Model;
using SimpleFEM.Core.Validation.Model.Rules;

namespace SimpleFEM.Core.Tests.Validation;

public class ReferenceIntegrityRuleTests
{
    private static readonly ReferenceIntegrityRule Rule = new();

    [Fact]
    public void Validate_AllReferencesResolve_ReturnsNoErrors()
    {
        var model = ModelBuilder.Build();

        Assert.Empty(Rule.Validate(model));
    }

    [Fact]
    public void Validate_ElementReferencesMissingNode_ReturnsUnknownReference()
    {
        var model = ModelBuilder.Build(elements: [new TrussElement2D(0, 0, 99, 0, 0)]);

        var error = Assert.Single(Rule.Validate(model));
        Assert.Equal(ModelValidationErrorCode.UnknownReference, error.Code);
    }

    [Fact]
    public void Validate_ElementReferencesMissingMaterial_ReturnsUnknownReference()
    {
        var model = ModelBuilder.Build(elements: [new TrussElement2D(0, 0, 1, 99, 0)]);

        var error = Assert.Single(Rule.Validate(model));
        Assert.Equal(ModelValidationErrorCode.UnknownReference, error.Code);
    }

    [Fact]
    public void Validate_ElementReferencesMissingSection_ReturnsUnknownReference()
    {
        var model = ModelBuilder.Build(elements: [new TrussElement2D(0, 0, 1, 0, 99)]);

        var error = Assert.Single(Rule.Validate(model));
        Assert.Equal(ModelValidationErrorCode.UnknownReference, error.Code);
    }

    [Fact]
    public void Validate_SupportReferencesMissingNode_ReturnsUnknownReference()
    {
        var model = ModelBuilder.Build(supports: [new Support(99, Restraint.Rigid())]);

        var error = Assert.Single(Rule.Validate(model));
        Assert.Equal(ModelValidationErrorCode.UnknownReference, error.Code);
    }

    [Fact]
    public void Validate_LoadReferencesMissingNode_ReturnsUnknownReference()
    {
        var model = ModelBuilder.Build(loads: [new NodalLoad(99, 10, 0, 0)]);

        var error = Assert.Single(Rule.Validate(model));
        Assert.Equal(ModelValidationErrorCode.UnknownReference, error.Code);
    }
}