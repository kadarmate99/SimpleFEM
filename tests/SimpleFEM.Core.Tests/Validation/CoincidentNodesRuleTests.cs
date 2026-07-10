using SimpleFEM.Core.Domain;
using SimpleFEM.Core.Validation.Model;
using SimpleFEM.Core.Validation.Model.Rules;

namespace SimpleFEM.Core.Tests.Validation;

public class CoincidentNodesRuleTests
{
    private const double MinDistance = 2;

    private static readonly CoincidentNodesRule Rule = new(MinDistance);

    [Fact]
    public void Validate_NodesFartherThanMinDistance_ReturnsNoErrors()
    {
        var model = ModelBuilder.Build(nodes: [new Node(0, 0, 0), new Node(1, 3.0, 0)]);

        Assert.Empty(Rule.Validate(model));
    }

    [Fact]
    public void Validate_NodesCloserThanMinDistance_ReturnsCoincidentNodes()
    {
        var model = ModelBuilder.Build(nodes: [new Node(0, 0, 0), new Node(1, 1.0, 0)]);

        var error = Assert.Single(Rule.Validate(model));
        Assert.Equal(ModelValidationErrorCode.CoincidentNodes, error.Code);
    }

    [Fact]
    public void Validate_NodesExactlyAtMinDistance_ReturnsNoErrors()
    {
        var model = ModelBuilder.Build(nodes: [new Node(0, 0, 0), new Node(1, MinDistance, 0)]);

        Assert.Empty(Rule.Validate(model));
    }
}
