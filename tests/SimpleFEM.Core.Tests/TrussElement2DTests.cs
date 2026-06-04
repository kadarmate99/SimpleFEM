using MathNet.Numerics.LinearAlgebra;
using SimpleFEM.Core.Domain;
using SimpleFEM.Core.Elements;

namespace SimpleFEM.Core.Tests
{
    public class TrussElement2DTests
    {
        [Fact]
        public void GlobalStiffness_MatrixIs4x4()
        {
            // Arrange
            var nodeI = new Node(0, 0.0, 0.0);
            var nodeJ = new Node(1, 2.0, 0.0);
            var material = new Material(0, "Steel", 200e9);
            var section = new CrossSection(0, "A", 0.01);

            var element = new TrussElement2D(0, nodeI.Id, nodeJ.Id, material.Id, section.Id);

            // Act
            var kg = element.ComputeGlobalStiffnessMatrix(nodeI, nodeJ, material, section);
            
            // Assert 
            Assert.Equal(4, kg.RowCount);
            Assert.Equal(4, kg.ColumnCount);
        }

        [Fact]
        public void GlobalStiffness_IsSymmetric()
        {
            // Arrange
            var nodeI = new Node(0, 0.0, 0.0);
            var nodeJ = new Node(1, 2.0, 0.0);
            var material = new Material(0, "Steel", 200e9);
            var section = new CrossSection(0, "A", 0.01);

            var element = new TrussElement2D(0, nodeI.Id, nodeJ.Id, material.Id, section.Id);

            // Act
            var kg = element.ComputeGlobalStiffnessMatrix(nodeI, nodeJ, material, section);

            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    Assert.Equal(kg[i, j], kg[j, i], precision: 10);
        }

        [Fact]
        public void GlobalStiffness_EachRowSumsToZero()
        {
            // Arrange
            var nodeI = new Node(0, 0.0, 0.0);
            var nodeJ = new Node(1, 2.0, 0.0);
            var material = new Material(0, "Steel", 200e9);
            var section = new CrossSection(0, "A", 0.01);

            var element = new TrussElement2D(0, nodeI.Id, nodeJ.Id, material.Id, section.Id);

            // Act
            var kg = element.ComputeGlobalStiffnessMatrix(nodeI, nodeJ, material, section);

            for (int i = 0; i < 4; i++)
            {
                double rowSum = kg.Row(i).Sum();
                Assert.Equal(0.0, rowSum, precision: 10);
            }
        }

        [Theory]
        [InlineData(0, 0, 3, 0)]
        [InlineData(0, 0, 0, 4)]
        [InlineData(0, 0, 3, 4)]
        [InlineData(-1, -1, 2, 3)]
        public void GlobalStiffness_MatchesHandCalculation(double x1, double y1, double x2, double y2)
        {
            // Arrange
            double E = 100.0;
            double A = 1.0;

            double dx = x2 - x1;
            double dy = y2 - y1;
            double L = Math.Sqrt(dx * dx + dy * dy);

            double c = dx / L;
            double s = dy / L;

            // (EA/L) * c²
            double ea = E * A / L;
            double kC2 = ea * c * c;
            double kS2 = ea * s * s;
            double kCS = ea * c * s;

            var expected = Matrix<double>.Build.DenseOfArray(new double[,]
            {
                {  kC2,  kCS, -kC2, -kCS },
                {  kCS,  kS2, -kCS, -kS2 },
                { -kC2, -kCS,  kC2,  kCS },
                { -kCS, -kS2,  kCS,  kS2 }
            });


            var nodeI = new Node(0, x1, y1);
            var nodeJ = new Node(1, x2, y2);
            var material = new Material(0, "Steel", E);
            var section = new CrossSection(0, "A", A);

            var element = new TrussElement2D(0, nodeI.Id, nodeJ.Id, material.Id, section.Id);

            // Act
            var kg = element.ComputeGlobalStiffnessMatrix(nodeI, nodeJ, material, section);
            
            // Assert
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    Assert.Equal(expected[i, j], kg[i, j], precision: 10);
        }

        [Fact]
        public void GlobalStiffness_ReversedStartEnd_ProducesIdenticalMatrix()
        {
            // Arrange
            var a = new Node(0, 0.0, 0.0);
            var b = new Node(1, 2.0, 0.0);
            var material = new Material(0, "Steel", 200e9);
            var section = new CrossSection(0, "A", 0.01);

            var forward = new TrussElement2D(0, a.Id, b.Id, material.Id, section.Id);
            var reversed = new TrussElement2D(0, b.Id, a.Id, material.Id, section.Id);

            // Act
            var kgF = forward.ComputeGlobalStiffnessMatrix(a, b, material, section);
            var kgR = reversed.ComputeGlobalStiffnessMatrix(b, a, material, section);

            // Assert
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    Assert.Equal(kgF[i, j], kgR[i, j]);
        }
    }
}
