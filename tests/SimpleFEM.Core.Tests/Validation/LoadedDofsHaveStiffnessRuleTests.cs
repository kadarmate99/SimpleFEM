using SimpleFEM.Core.Domain;
using SimpleFEM.Core.Domain.Supports;
using SimpleFEM.Core.Elements;
using SimpleFEM.Core.Loads;
using SimpleFEM.Core.Validation.Model;
using SimpleFEM.Core.Validation.Model.Rules;

namespace SimpleFEM.Core.Tests.Validation;

public class LoadedDofsHaveStiffnessRuleTests
{
    private static readonly LoadedDofsHaveStiffnessRule Rule = new();

    [Fact]
    public void Validate_LoadOnDofWithElementStiffness_ReturnsNoErrors()
    {
        var model = ModelBuilder.Build(
            nodes: [new Node(0, 0, 0), new Node(1, 1, 0)],
            elements: [new TrussElement2D(0, 0, 1, 0, 0)],
            supports: [], // only elements no supports
            loads: [new NodalLoad(0, -5, -5, 0), new NodalLoad(1, 5, 5, 0)]);

        Assert.Empty(Rule.Validate(model));
    }

    [Fact]
    public void Validate_MomentLoadWithoutRotationalStiffness_ReturnsUnsupportedDof()
    {
        var model = ModelBuilder.Build(
            nodes: [new Node(0, 0, 0), new Node(1, 1, 0)],
            elements: [new TrussElement2D(0, 0, 1, 0, 0)],
            supports:
            [
                new Support(0, ux: Restraint.Rigid(), uy: Restraint.Elastic(1e4)),
                new Support(1, uy: Restraint.Rigid()),
            ],
            loads: [new NodalLoad(1, 0, 0, 5.0)]); // moment on truss element and no rotational support

        var error = Assert.Single(Rule.Validate(model));
        Assert.Equal(ModelValidationErrorCode.UnsupportedDof, error.Code);
    }

    [Fact]
    public void Validate_LoadsOnSupportDofs_ReturnsNoErrors()
    {
        var model = ModelBuilder.Build(
            elements: [], // no elements only support
            loads: [new NodalLoad(0, 5, 5, 5), new NodalLoad(1, 5, 5, 5),],
            supports:
            [
                new Support(0, Restraint.Rigid(), Restraint.Rigid(), Restraint.Rigid()),
                new Support(1,  Restraint.Elastic(1e4), Restraint.Elastic(1e4), Restraint.Elastic(1e4)),
            ]);

        Assert.Empty(Rule.Validate(model));
    }
}
