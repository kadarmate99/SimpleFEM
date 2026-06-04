using SimpleFEM.Core.Analysis;
using SimpleFEM.Core.Domain;
using SimpleFEM.Core.Domain.Supports;
using SimpleFEM.Core.Elements;
using SimpleFEM.Core.Loads;

namespace SimpleFEM.Core.Tests
{
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

            var elements = new List<ILineElement>
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

            var result = new FemAnalysis().Run(model);

            var f0 = result.ElementInternalForces.Single(f => f.ElementId == elementId);
            Assert.Equal(-10, f0.N, tolerance: 1e-6);
            Assert.Equal(0, f0.V, tolerance: 1e-6);
            Assert.Equal(0, f0.M, tolerance: 1e-6);
        }

        [Fact]
        public void Run_NodalDisplacements_MatchHandCalculation()
        {
            double e = 210_000_000_000;
            double a = 1;
            double l = 5;
            var model = BuildVerticalTrussModel(e, a, l);

            var result = new FemAnalysis().Run(model);

            var d0 = result.NodalDisplacements.Single(d => d.NodeId == 0);
            Assert.Equal(0, d0.Ux, tolerance: 1e-6);
            Assert.Equal(0, d0.Uy, tolerance: 1e-6);
            Assert.Equal(0, d0.Rz, tolerance: 1e-6);

            var d1 = result.NodalDisplacements.Single(d => d.NodeId == 1);
            double expectedUy1 = -10.0 / (e * a / l);
            Assert.Equal(0, d1.Ux, tolerance: 1e-6);
            Assert.Equal(expectedUy1, d1.Uy, tolerance: 1e-6);
            Assert.Equal(0, d1.Rz, tolerance: 1e-6);
        }

        [Fact]
        public void Run_Reactions_BalanceAppliedLoads()
        {
            var model = BuildVerticalTrussModel(210_000_000_000, 1, 5);

            var result = new FemAnalysis().Run(model);

            // node 0: Fy = 1 - 10 = -9; R = -F
            var r0 = result.Reactions.Single(r => r.NodeId == 0);
            Assert.Equal(0, r0.Rx, tolerance: 1e-6);
            Assert.Equal(9, r0.Ry, tolerance: 1e-6);
            Assert.Equal(0, r0.Mz, tolerance: 1e-6);

            // node 1: Fx = 1; R = -F
            var r1 = result.Reactions.Single(r => r.NodeId == 1);
            Assert.Equal(-1, r1.Rx, tolerance: 1e-6);
            Assert.Equal(0, r1.Ry, tolerance: 1e-6);
            Assert.Equal(0, r1.Mz, tolerance: 1e-6);
        }

        [Fact]
        public void SimplySupportedTriangle_MatchHandCalculation()
        {
            var nodes = new List<Node> { new(0, 0, 0), new(1, 6, 0), new(2, 3, 4) };
            var materials = new List<Material> { new(0, "S235", 210e9) };
            var sections = new List<CrossSection> { new(0, "A", 1e-3) };
            var elements = new List<ILineElement>
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

            var result = new FemAnalysis().Run(model);

            Assert.Multiple(
                () => Assert.Equal(5, result.Reactions.Single(r => r.NodeId == 0).Ry, 1e-6),
                () => Assert.Equal(0, result.Reactions.Single(r => r.NodeId == 0).Rx, 1e-6),
                () => Assert.Equal(5, result.Reactions.Single(r => r.NodeId == 1).Ry, 1e-6),
                () => Assert.Equal(3.75, result.ElementInternalForces.Single(f => f.ElementId == 0).N, 1e-6),
                () => Assert.Equal(-6.25, result.ElementInternalForces.Single(f => f.ElementId == 1).N, 1e-6),
                () => Assert.Equal(-6.25, result.ElementInternalForces.Single(f => f.ElementId == 2).N, 1e-6));
        }
    }
}

