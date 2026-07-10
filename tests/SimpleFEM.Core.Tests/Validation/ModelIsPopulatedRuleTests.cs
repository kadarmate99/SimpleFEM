using SimpleFEM.Core.Loads;
using SimpleFEM.Core.Validation.Model;
using SimpleFEM.Core.Validation.Model.Rules;

namespace SimpleFEM.Core.Tests.Validation;

public class ModelIsPopulatedRuleTests
{
    private static readonly ModelIsPopulatedRule Rule = new();

    [Fact]
    public void Validate_PopulatedModel_ReturnsNoErrors()
    {
        var model = ModelBuilder.Build();

        Assert.Empty(Rule.Validate(model));
    }

    [Fact]
    public void Validate_EmptyModel_ReportsMissingNodesElementsSupportsAndLoads()
    {
        var model = ModelBuilder.Build(
            nodes: [], materials: [], sections: [], elements: [], loads: [], supports: []);

        var errors = Rule.Validate(model).ToList();

        Assert.Equal(4, errors.Count);
        Assert.All(errors, e => Assert.Equal(ModelValidationErrorCode.EmptyModel, e.Code));
    }

    [Fact]
    public void Validate_NoLoads_ReturnsSingleEmptyModelError()
    {
        var model = ModelBuilder.Build(loads: Array.Empty<NodalLoad>());

        var error = Assert.Single(Rule.Validate(model));
        Assert.Equal(ModelValidationErrorCode.EmptyModel, error.Code);
    }
}