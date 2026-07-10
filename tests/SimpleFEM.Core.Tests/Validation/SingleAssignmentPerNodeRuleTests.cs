using SimpleFEM.Core.Domain.Supports;
using SimpleFEM.Core.Loads;
using SimpleFEM.Core.Validation.Model;
using SimpleFEM.Core.Validation.Model.Rules;

namespace SimpleFEM.Core.Tests.Validation;

public class SingleAssignmentPerNodeRuleTests
{
    private static readonly SingleAssignmentPerNodeRule Rule = new();

    [Fact]
    public void Validate_AtMostOneSupportAndLoadPerNode_ReturnsNoErrors()
    {
        var model = ModelBuilder.Build();

        Assert.Empty(Rule.Validate(model));
    }

    [Fact]
    public void Validate_TwoSupportsOnSameNode_ReturnsDuplicateNodeAssignment()
    {
        var model = ModelBuilder.Build(supports:
        [
            new Support(0, Restraint.Rigid(), Restraint.Rigid()),
            new Support(0, uy: Restraint.Rigid()),
        ]);

        var error = Assert.Single(Rule.Validate(model));
        Assert.Equal(ModelValidationErrorCode.DuplicateNodeAssignment, error.Code);
    }

    [Fact]
    public void Validate_TwoLoadsOnSameNode_ReturnsDuplicateNodeAssignment()
    {
        var model = ModelBuilder.Build(loads:
        [
            new NodalLoad(1, 10, 0, 0),
            new NodalLoad(1, 0, 5, 0),
        ]);

        var error = Assert.Single(Rule.Validate(model));
        Assert.Equal(ModelValidationErrorCode.DuplicateNodeAssignment, error.Code);
    }
}


