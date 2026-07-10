using SimpleFEM.Core.Analysis;
using SimpleFEM.Core.Domain;
using SimpleFEM.Core.Domain.Supports;
using SimpleFEM.Core.Elements;
using SimpleFEM.Core.Exceptions;
using SimpleFEM.Core.Loads;
using SimpleFEM.Core.Tests.Validation;
using SimpleFEM.Core.Validation.Model;

namespace SimpleFEM.Core.Tests.Analysis;

public class FemAnalysisTests
{

    private static FemModel BuildVerticalTrussModel(double e, double a, double l)
    {
        var nodes = new List<Node>
        {
            new Node(0, 0, 0),
            new Node(1, 0, l)
        };

        var materials = new List<Material> { new Material(0, $"testMat_{e}", e) };
        var sections = new List<CrossSection> { new CrossSection(0, $"testSec_{a}", a) };

        var elements = new List<Element>
        {
            new TrussElement2D(0, 0, 1, 0, 0)
        };

        var loads = new List<NodalLoad>
        {
            new NodalLoad(0, 0, 1, 0),
            new NodalLoad(1, 1, -10, 0)
        };

        var supports = new List<Support>
        {
            // pined
            new Support(0, Restraint.Rigid(), Restraint.Rigid(), null),

            // vertical roller
            new Support(1, Restraint.Rigid(), null, null)
        };

        return new FemModel(nodes, materials, sections, elements, loads, supports);
    }


    [Fact]
    public void Run_ElementAxialForce_MatchesHandCalculation()
    {
        var model = BuildVerticalTrussModel(210_000_000_000, 1, 5);
        var elementId = 0;

        var result = new FemAnalysis().Run(model).EnsureValid();

        var f0 = result.ElementInternalForces.Single(f => f.ElementId == elementId);
        Assert.Equal(-10, f0.N, Tolerances.Tol);
        Assert.Equal(0, f0.V, Tolerances.Tol);
        Assert.Equal(0, f0.M, Tolerances.Tol);
    }

    [Fact]
    public void Run_NodalDisplacements_MatchHandCalculation()
    {
        double e = 210_000_000_000;
        double a = 1;
        double l = 5;
        var model = BuildVerticalTrussModel(e, a, l);

        var result = new FemAnalysis().Run(model).EnsureValid();

        var d0 = result.NodalDisplacements.Single(d => d.NodeId == 0);
        Assert.Equal(0, d0.Ux, Tolerances.Tol);
        Assert.Equal(0, d0.Uy, Tolerances.Tol);
        Assert.Equal(0, d0.Rz, Tolerances.Tol);

        var d1 = result.NodalDisplacements.Single(d => d.NodeId == 1);
        double expectedUy1 = -10.0 / (e * a / l);
        Assert.Equal(0, d1.Ux, Tolerances.Tol);
        Assert.Equal(expectedUy1, d1.Uy, Tolerances.Tol);
        Assert.Equal(0, d1.Rz, Tolerances.Tol);
    }

    [Fact]
    public void Run_Reactions_BalanceAppliedLoads()
    {
        var model = BuildVerticalTrussModel(210_000_000_000, 1, 5);

        var result = new FemAnalysis().Run(model).EnsureValid();

        // node 0: Fy = 1 - 10 = -9; R = -F
        var r0 = result.Reactions.Single(r => r.NodeId == 0);
        Assert.Equal(0, r0.Rx, Tolerances.Tol);
        Assert.Equal(9, r0.Ry, Tolerances.Tol);
        Assert.Equal(0, r0.Mz, Tolerances.Tol);

        // node 1: Fx = 1; R = -F
        var r1 = result.Reactions.Single(r => r.NodeId == 1);
        Assert.Equal(-1, r1.Rx, Tolerances.Tol);
        Assert.Equal(0, r1.Ry, Tolerances.Tol);
        Assert.Equal(0, r1.Mz, Tolerances.Tol);
    }

    [Fact]
    public void Run_SimplySupportedTriangle_MatchHandCalculation()
    {
        var nodes = new List<Node> { new(0, 0, 0), new(1, 6, 0), new(2, 3, 4) };
        var materials = new List<Material> { new(0, "S235", 210e9) };
        var sections = new List<CrossSection> { new(0, "A", 1e-3) };
        var elements = new List<Element>
        {
            new TrussElement2D(0, 0, 1, 0, 0),
            new TrussElement2D(1, 0, 2, 0, 0),
            new TrussElement2D(2, 1, 2, 0, 0),
        };
        var loads = new List<NodalLoad> { new(2, 0, -10, 0) };
        var supports = new List<Support>
        {
            new(0, ux: Restraint.Rigid(), uy: Restraint.Rigid()),
            new(1, uy: Restraint.Rigid()),
        };
        var model = new FemModel(nodes, materials, sections, elements, loads, supports);

        var result = new FemAnalysis().Run(model).EnsureValid();

        Assert.Multiple(
            () => Assert.Equal(5, result.Reactions.Single(r => r.NodeId == 0).Ry, Tolerances.Tol),
            () => Assert.Equal(0, result.Reactions.Single(r => r.NodeId == 0).Rx, Tolerances.Tol),
            () => Assert.Equal(5, result.Reactions.Single(r => r.NodeId == 1).Ry, Tolerances.Tol),
            () => Assert.Equal(3.75, result.ElementInternalForces.Single(f => f.ElementId == 0).N, Tolerances.Tol),
            () => Assert.Equal(-6.25, result.ElementInternalForces.Single(f => f.ElementId == 1).N, Tolerances.Tol),
            () => Assert.Equal(-6.25, result.ElementInternalForces.Single(f => f.ElementId == 2).N, Tolerances.Tol));
    }

    [Fact]
    public void Run_FreeNode_ProducesNoReaction()
    {
        var nodes = new List<Node> { new(0, 0, 0), new(1, 6, 0), new(2, 3, 4) };
        var materials = new List<Material> { new(0, "S235", 210e9) };
        var sections = new List<CrossSection> { new(0, "A", 1e-3) };
        var elements = new List<Element>
        {
            new TrussElement2D(0, 0, 1, 0, 0),
            new TrussElement2D(1, 0, 2, 0, 0),
            new TrussElement2D(2, 1, 2, 0, 0),
        };
        var loads = new List<NodalLoad> { new(2, 0, -10, 0) };
        var supports = new List<Support>
        {
            new(0, ux: Restraint.Rigid(), uy: Restraint.Rigid()),
            new(1, uy: Restraint.Rigid()),
        };
        var model = new FemModel(nodes, materials, sections, elements, loads, supports);

        var result = new FemAnalysis().Run(model).EnsureValid();

        // node 2 has no support
        Assert.DoesNotContain(result.Reactions, r => r.NodeId == 2);
    }


    [Fact]
    public void Run_InvalidModel_ThrowsInvalidModelExceptionCarryingValidation()
    {
        // load references a missing node (ReferenceIntegrity).
        var model = ModelBuilder.Build(loads: [new NodalLoad(99, 10, 0, 0)]);

        var ex = Assert.Throws<InvalidModelException>(() => new FemAnalysis().Run(model));

        Assert.False(ex.Validation.IsValid);
        Assert.Same(model, ex.Model);
    }

    [Fact]
    public void Validate_ValidModel_IsValidWithNoErrors()
    {
        var result = new FemAnalysis().Validate(ModelBuilder.Build());

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void Validate_ModelBreakingMultipleRules_AccumulatesErrorsFromEachRule()
    {
        // load references a missing node (ReferenceIntegrity).
        // two supports on node 0 (SingleAssignmentPerNode).
        var model = ModelBuilder.Build(
            loads: [new NodalLoad(99, 10, 0, 0)],
            supports:
            [
                new Support(0, Restraint.Rigid(), Restraint.Rigid()),
                new Support(0, uy: Restraint.Rigid()),
            ]);

        var result = new FemAnalysis().Validate(model);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Code == ModelValidationErrorCode.UnknownReference);
        Assert.Contains(result.Errors, e => e.Code == ModelValidationErrorCode.DuplicateNodeAssignment);
    }
}