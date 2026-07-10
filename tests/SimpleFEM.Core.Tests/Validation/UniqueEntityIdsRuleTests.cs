using SimpleFEM.Core.Domain;
using SimpleFEM.Core.Elements;
using SimpleFEM.Core.Validation.Model;
using SimpleFEM.Core.Validation.Model.Rules;

namespace SimpleFEM.Core.Tests.Validation;

public class UniqueEntityIdsRuleTests
{
    private static readonly UniqueEntityIdsRule Rule = new();

    [Fact]
    public void Validate_UniqueIds_ReturnsNoErrors()
    {

        var model = ModelBuilder.Build(
            nodes: [new Node(0, 0, 0), new Node(1, 1, 0), new Node(2, 2, 0)],
            elements: [new TrussElement2D(0, 0, 1, 0, 0), new TrussElement2D(1, 1, 2, 0, 0)],
            sections: [new CrossSection(0, "A", 1), new CrossSection(1, "B", 2)],
            materials: [new Material(0, "A", 1), new Material(1, "B", 2)]);

        Assert.Empty(Rule.Validate(model));
    }

    [Fact]
    public void Validate_DuplicateElementIds_ReturnsDuplicateIdsError()
    {
        var model = ModelBuilder.Build(
            nodes: [new Node(0, 0, 0), new Node(1, 1, 0), new Node(2, 2, 0)],
            elements: [new TrussElement2D(0, 0, 1, 0, 0), new TrussElement2D(0, 1, 2, 0, 0)]);

        var error = Assert.Single(Rule.Validate(model));
        Assert.Equal(ModelValidationErrorCode.DuplicateIds, error.Code);
    }

    [Fact]
    public void Validate_DuplicateNodeIds_ReturnsDuplicateIdsError()
    {
        var model = ModelBuilder.Build(nodes: [new Node(0, 0, 0), new Node(0, 1, 0)]);

        var error = Assert.Single(Rule.Validate(model));
        Assert.Equal(ModelValidationErrorCode.DuplicateIds, error.Code);
    }

    [Fact]
    public void Validate_DuplicateSectionIds_ReturnsDuplicateIdsError()
    {
        var model = ModelBuilder.Build(sections: [new CrossSection(0, "A", 1), new CrossSection(0, "B", 2)]);

        var error = Assert.Single(Rule.Validate(model));
        Assert.Equal(ModelValidationErrorCode.DuplicateIds, error.Code);
    }

    [Fact]
    public void Validate_DuplicateMaterialIds_ReturnsDuplicateIdsError()
    {
        var model = ModelBuilder.Build(materials: [new Material(0, "A", 1), new Material(0, "B", 2)]);

        var error = Assert.Single(Rule.Validate(model));
        Assert.Equal(ModelValidationErrorCode.DuplicateIds, error.Code);
    }
}
