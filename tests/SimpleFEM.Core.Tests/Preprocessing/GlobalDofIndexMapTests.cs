using SimpleFEM.Core.Domain;
using SimpleFEM.Core.Elements;
using SimpleFEM.Core.Preprocessing;

namespace SimpleFEM.Core.Tests.Preprocessing
{
    public class GlobalDofIndexMapTests
    {
        [Fact]
        public void MapsAllActiveDofsToUniqueGlobalIndices()
        {
            var nodes = new[] { new Node(0, 0, 0), new Node(1, 1, 0) };
            var elements = new ILineElement[] { new TrussElement2D(0, 0, 1, 0, 0) };

            var map = new GlobalDofIndexMap(nodes, elements);

            // 2D truss has Ux Uy at nodes
            var expectedDofs = new[]
            {
                new Dof(0, DofType.Ux), new Dof(0, DofType.Uy),
                new Dof(1, DofType.Ux), new Dof(1, DofType.Uy),
            };
            int[] expectedIndices = [0, 1, 2, 3];

            List<int> indices = new();
            foreach (var dof in expectedDofs)
                indices.Add(map.GlobalIndexOf(dof));

            Assert.Equal(expectedDofs.Length, map.ActiveDofCount);
            Assert.Equal(map.ActiveDofCount, indices.Distinct().Count());
            Assert.Equal(expectedIndices, indices);
        }

        [Fact]
        public void ExcludesUnreferencedNodes_AndThrowsForInactiveDof()
        {
            var nodes = new[] { new Node(0, 0, 0), new Node(1, 1, 0), new Node(2, 2, 0) };
            var elements = new ILineElement[] { new TrussElement2D(0, 0, 1, 0, 0) };

            var map = new GlobalDofIndexMap(nodes, elements);

            Assert.Equal(4, map.ActiveDofCount); // only nodes 0 and 1 is active
            Assert.Throws<ArgumentException>(() => map.GlobalIndexOf(new Dof(2, DofType.Ux)));
        }
    }
}
