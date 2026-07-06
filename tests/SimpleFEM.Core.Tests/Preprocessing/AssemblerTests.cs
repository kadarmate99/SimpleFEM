using MathNet.Numerics.LinearAlgebra;
using SimpleFEM.Core.Domain;
using SimpleFEM.Core.Domain.Supports;
using SimpleFEM.Core.Elements;
using SimpleFEM.Core.Loads;
using SimpleFEM.Core.Preprocessing;

namespace SimpleFEM.Core.Tests.Preprocessing;

public class AssemblerTests
{
    private const double E = 100;
    private const double A = 1;
    private const double K = E * A / 1; //  k = EA/L

    // three nodes horizontaly at 1m
    // two elements, node 1 shared
    private static FemModel BuildHorizontalChain(IReadOnlyList<NodalLoad>? loads = null) =>
        new(
            new List<Node> { new(0, 0, 0), new(1, 1, 0), new(2, 2, 0) },
            new List<Material> { new(0, "m", 100) },
            new List<CrossSection> { new(0, "a", 1.0) },
            new List<Element>
            {
                new TrussElement2D(0, 0, 1, 0, 0),
                new TrussElement2D(1, 1, 2, 0, 0),
            },
            loads ?? new List<NodalLoad>(),
            new List<Support>());

    [Fact]
    public void Assemble_SharedDof_AddsElementContributions()
    {
        var model = BuildHorizontalChain();
        var dofMap = new GlobalDofIndexMap(model.Nodes, model.Elements);
        var system = new Assembler().Assemble(model, dofMap);

        int n0 = dofMap.GlobalIndexOf(new Dof(0, DofType.Ux));
        int n1 = dofMap.GlobalIndexOf(new Dof(1, DofType.Ux));
        int n2 = dofMap.GlobalIndexOf(new Dof(2, DofType.Ux));

        // shared DOF adds both elements 
        Assert.Equal(2 * K, system.K[n1, n1], Tolerances.Tol);

        // end DOFs add one element
        Assert.Equal(K, system.K[n0, n0], Tolerances.Tol);
        Assert.Equal(K, system.K[n2, n2], Tolerances.Tol);

        // the two end DOFs have no connection
        Assert.Equal(0.0, system.K[n0, n2], Tolerances.Tol);
        Assert.Equal(0.0, system.K[n0, n2], Tolerances.Tol);
    }

    [Fact]
    public void Assemble_PlacesLoadAtCorrectGlobalDof()
    {
        var model = BuildHorizontalChain(new List<NodalLoad> { new(1, 7, 0, 0) });
        var dofMap = new GlobalDofIndexMap(model.Nodes, model.Elements);
        var system = new Assembler().Assemble(model, dofMap);

        int n1ux = dofMap.GlobalIndexOf(new Dof(1, DofType.Ux));

        Assert.Equal(7, system.F[n1ux], Tolerances.Tol);
        Assert.Equal(7, system.F.Sum(), Tolerances.Tol); // nothing placed elsewhere
    }

    [Fact]
    public void Assemble_K_IsSymmetric()
    {
        var model = BuildHorizontalChain();
        var dofMap = new GlobalDofIndexMap(model.Nodes, model.Elements);
        var k = new Assembler().Assemble(model, dofMap).K;

        Assert.Equal(k.ColumnCount, k.RowCount);

        for (int i = 0; i < k.RowCount; i++)
            for (int j = 0; j < k.ColumnCount; j++)
                Assert.Equal(k[i, j], k[j, i]);
    }

    [Fact]
    public void Assemble_K_HasNonNegativeDiagonal()
    {
        var model = BuildHorizontalChain();
        var dofMap = new GlobalDofIndexMap(model.Nodes, model.Elements);
        var k = new Assembler().Assemble(model, dofMap).K;

        for (int i = 0; i < k.RowCount; i++)
            Assert.True(k[i, i] >= 0);
    }

    [Fact]
    public void Assemble_K_IsPositiveSemiDefinite()
    {
        var model = BuildHorizontalChain();
        var dofMap = new GlobalDofIndexMap(model.Nodes, model.Elements);
        var k = new Assembler().Assemble(model, dofMap).K;

        // positive semidefinite =  all eigenvalues ≥ 0
        var eigenvalues = k.Evd().EigenValues.Real();
        double minEig = eigenvalues.Min();

        Assert.True(minEig >= 0);
    }
}
