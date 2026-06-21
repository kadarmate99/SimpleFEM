using SimpleFEM.Core.Analysis;
using SimpleFEM.Core.Domain;
using SimpleFEM.Core.Domain.Supports;
using SimpleFEM.Core.Elements;
using SimpleFEM.Core.Loads;

namespace SimpleFEM.Core.IntegrationTests.TrussConsole;

public class TrussConsoleTests
{
    private const double E = 210e9; // Pa

    private const double AreaHea300 = 0.011253; //m2
    private const double AreaShs150 = 0.002873;

    private const double DisplacementTolM = 1e-4; // 0.1 mm
    private const double ForceTolN = 0.1;


    private const int MatS235 = 0, MatS355 = 1;
    private const int SecHea300 = 0, SecShs150 = 1;

    private static FemModel BuildTrussConsoleModel()
    {
        var nodes = new List<Node>
        {
            new(1, 0,  0),
            new(2, 10, 0),
            new(3, 0,  2),
            new(4, 10, 2),
            new(5, 2,  0),
            new(6, 4,  2),
            new(7, 6,  0),
            new(8, 8,  2),
        };

        var materials = new List<Material>
        {
            new(MatS235, "S 235", E),
            new(MatS355, "S 355", E),
        };

        var sections = new List<CrossSection>
        {
            new(SecHea300, "HEA 300",   AreaHea300),
            new(SecShs150, "SHS 150x5", AreaShs150),
        };

        var elements = new List<ILineElement>
        {
            new TrussElement2D(1,  2, 4, MatS355, SecShs150),
            new TrussElement2D(2,  3, 5, MatS355, SecShs150),
            new TrussElement2D(3,  1, 5, MatS235, SecHea300),
            new TrussElement2D(4,  5, 6, MatS355, SecShs150),
            new TrussElement2D(5,  3, 6, MatS235, SecHea300),
            new TrussElement2D(6,  6, 7, MatS355, SecShs150),
            new TrussElement2D(7,  7, 2, MatS235, SecHea300),
            new TrussElement2D(8,  5, 7, MatS235, SecHea300),
            new TrussElement2D(9,  7, 8, MatS355, SecShs150),
            new TrussElement2D(10, 8, 4, MatS235, SecHea300),
            new TrussElement2D(11, 6, 8, MatS235, SecHea300),
            new TrussElement2D(12, 8, 2, MatS355, SecShs150),
        };

        var loads = new List<NodalLoad>
        {
            new(4, 50_000,  -100_000, 0),
            new(5, 10_000,  -50_000,  0),
            new(6, -10_000, 125_000,  0),
        };

        var supports = new List<Support>
        {
            new(1, ux: Restraint.Rigid(), uy: Restraint.Rigid()),
            new(3, ux: Restraint.Rigid(), uy: Restraint.Rigid()),
        };

        return new FemModel(nodes, materials, sections, elements, loads, supports);
    }

    [Fact]
    public void Run_TrussConsole_NodalDisplacements_MatchReference()
    {
        var result = new FemAnalysis().Run(BuildTrussConsoleModel());

        // in mm
        var expected = new (int NodeId, double UxMm, double UyMm)[]
        {
            (1,  0.000,  0.000),
            (2, -0.923, -8.712),
            (3,  0.000,  0.000),
            (4,  0.999, -9.043),
            (5, -0.245, -0.480),
            (6,  0.533, -1.024),
            (7, -0.753, -3.248),
            (8,  0.956, -5.895),
        };

        foreach (var e in expected)
        {
            var d = result.NodalDisplacements.Single(d => d.NodeId == e.NodeId);
            Assert.Equal(e.UxMm/1000, d.Ux, DisplacementTolM);
            Assert.Equal(e.UyMm/1000, d.Uy, DisplacementTolM);
        }
    }

    [Fact]
    public void Run_TrussConsole_AxialForces_MatchReference()
    {
        var result = new FemAnalysis().Run(BuildTrussConsoleModel());

        // in N
        var fExpected = new (int ElementId, double N)[]
        {
            (1,  -100000),
            (2,    35355.34),
            (3,  -290000),
            (4,    35355.34),
            (5,   315000),
            (6,   141421.4),
            (7,  -100000),
            (8,  -300000),
            (9,  -141421.4),
            (10,   50000),
            (11,  250000),
            (12,  141421.4),
        };

        foreach (var e in fExpected)
        {
            var f = result.ElementInternalForces.Single(f => f.ElementId == e.ElementId);
            Assert.Equal(e.N, f.N, ForceTolN);
            Assert.Equal(0, f.V, ForceTolN);
            Assert.Equal(0, f.M, ForceTolN);
        }
    }

    [Fact]
    public void Run_TrussConsole_Reactions_MatchReference()
    {
        var result = new FemAnalysis().Run(BuildTrussConsoleModel());

        var r1 = result.Reactions.Single(r => r.NodeId == 1);
        Assert.Equal(290 * 1000, r1.Rx, ForceTolN);
        Assert.Equal(0, r1.Ry, ForceTolN);

        var r3 = result.Reactions.Single(r => r.NodeId == 3);
        Assert.Equal(-340 * 1000, r3.Rx, ForceTolN);
        Assert.Equal(25 * 1000, r3.Ry, ForceTolN);

        double sumRx = result.Reactions.Sum(r => r.Rx);
        double sumRy = result.Reactions.Sum(r => r.Ry);
        Assert.Equal(-50 * 1000, sumRx, ForceTolN);
        Assert.Equal(25 * 1000, sumRy, ForceTolN);
    }
}
