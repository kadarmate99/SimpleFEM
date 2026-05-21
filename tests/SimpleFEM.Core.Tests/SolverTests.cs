using SimpleFEM.Domain.Entities;
using SimpleFEM.Domain.ValueObjects;

namespace SimpleFEM.Core.Tests
{
    public class SolverTests
    {
        [Fact]
        public void Solve_ReturnsExpectedAxialForce()
        {
            // Arrange
            var nodes = new List<Node>
            {
                new Node(Guid.NewGuid(), new Coordinate2D(0, 0)),
                new Node(Guid.NewGuid(), new Coordinate2D(0, 5.0))
            };

            var materials = new List<Material>
            {
                new Material(Guid.NewGuid(), "S235", 210_000_000_000.0)
            };

            var sections = new List<Section>
            {
                new Section(Guid.NewGuid(), "1×1m", 1)
            };

            var elements = new List<TrussElement>
            {
                new TrussElement(Guid.NewGuid(), nodes[0].Id, nodes[1].Id, materials[0].Id, sections[0].Id)
            };

            var supports = new List<Support>
            {
                new Support(Guid.NewGuid(),nodes[0].Id, true, true),
                new Support(Guid.NewGuid(),nodes[1].Id, true, false),
            };

            var loads = new List<NodalLoad>
            {
                new NodalLoad(nodes[0].Id, new Force2D(0, 1)),
                new NodalLoad(nodes[1].Id, new Force2D(1, -10))
            };

            var loadCases = new List<LoadCase>
            {
                new LoadCase(Guid.NewGuid(), "test", loads)
            };

            var structure = new Structure(Guid.NewGuid(), nodes, elements, materials, sections, supports, loadCases);
            var solver = new Solver();

            // Act
            var result = solver.Solve(structure, structure.LoadCases[0]);

            // Assert
            var d0 = result.NodalDisplacements.Single(d => d.NodeId == nodes[0].Id);
            Assert.Equal(0.0, d0.Ux);
            Assert.Equal(0.0, d0.Uy);

            // F / (EA/L)
            var d1 = result.NodalDisplacements.Single(d => d.NodeId == nodes[1].Id);
            double expectedUy1 = -10.0 / (210_000_000_000.0 / 5.0);
            Assert.Equal(0.0, d1.Ux);
            Assert.Equal(expectedUy1, d1.Uy);

            var r0 = result.Reactions.Single(r => r.SupportId == supports[0].Id);
            Assert.Equal(0.0, r0.Rx);
            Assert.Equal(9.0, r0.Ry);

            var r1 = result.Reactions.Single(r => r.SupportId == supports[1].Id);
            Assert.Equal(-1.0, r1.Rx);
            Assert.Equal(0.0, r1.Ry);

            var f0 = result.ElementForces.Single(f => f.TrussElementId == elements[0].Id);
            Assert.Equal(-10.0, f0.AxialForce);
        }
    }
}
