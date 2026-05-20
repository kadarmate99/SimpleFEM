using MathNet.Numerics.LinearAlgebra;
using SimpleFEM.Core.Elements;
using SimpleFEM.Domain.Entities;
using SimpleFEM.Domain.ValueObjects;

namespace SimpleFEM.Core.Tests
{
    public class TrussElement2DTests
    {
        private static Node NodeAt(double x, double y) => new(Guid.NewGuid(), new Coordinate2D(x, y));
        private static Material Mat(double E) => new(Guid.NewGuid(), $"E={E}", E);
        private static Section Sec(double A) => new(Guid.NewGuid(), $"A={A}", A);

        #region Constructor
        [Fact]
        public void Constructor_NullStart_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new TrussElement2D(null!, NodeAt(1, 0), Mat(1), Sec(1)));
        }

        [Fact]
        public void Constructor_NullEnd_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new TrussElement2D(NodeAt(0, 0), null!, Mat(1), Sec(1)));
        }

        [Fact]
        public void Constructor_NullMaterial_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new TrussElement2D(NodeAt(0, 0), NodeAt(1, 0), null!, Sec(1)));
        }

        [Fact]
        public void Constructor_NullSection_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new TrussElement2D(NodeAt(0, 0), NodeAt(1, 0), Mat(1), null!));
        }

        [Fact]
        public void Constructor_InvalidLength_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
                new TrussElement2D(NodeAt(2, 3), NodeAt(2, 3), Mat(1), Sec(1)));
        }
        #endregion

        [Theory]
        [InlineData(0, 0, 3, 0, 3.0)]
        [InlineData(0, 0, 0, 4, 4.0)]
        [InlineData(0, 0, 3, 4, 5.0)]
        [InlineData(-1, -1, 2, 3, 5.0)]
        public void L_ReturnsCorrectLength(double x1, double y1, double x2, double y2, double expectedL)
        {
            var element = new TrussElement2D(NodeAt(x1, y1), NodeAt(x2, y2), Mat(1), Sec(1));

            Assert.Equal(expectedL, element.L);
        }

        #region LocalStiffness
        [Fact]
        public void LocalStiffness_MatrixIs4x4()
        {
            var element = new TrussElement2D(NodeAt(0, 0), NodeAt(1, 0), Mat(1), Sec(1));

            var k = element.LocalStiffnessMatrix;

            Assert.Equal(4, k.RowCount);
            Assert.Equal(4, k.ColumnCount);
        }

        [Fact]
        public void LocalStiffness_IsSymmetric()
        {
            var element = new TrussElement2D(NodeAt(0, 0), NodeAt(3, 4), Mat(1), Sec(1));

            var k = element.LocalStiffnessMatrix;

            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    Assert.Equal(k[i, j], k[j, i]);
        }

        [Fact]
        public void LocalStiffness_EachRowSumsToZero()
        {
            // Applying equal displacement to all DOFs is a rigid-body translation,
            // which produces zero net force. That means each row must sum to 0.
            var element = new TrussElement2D(NodeAt(0, 0), NodeAt(3, 4), Mat(1), Sec(1));

            var k = element.LocalStiffnessMatrix;

            for (int i = 0; i < 4; i++)
            {
                double rowSum = k.Row(i).Sum();
                Assert.Equal(0.0, rowSum);
            }
        }

        [Theory]
        [InlineData(0, 0, 3, 0)]
        [InlineData(0, 0, 0, 4)]
        [InlineData(0, 0, 3, 4)]
        [InlineData(-1, -1, 2, 3)]
        public void LocalStiffness_MatchesHandCalculation(double x1, double y1, double x2, double y2)
        {
            // Arrange
            double E = 100.0;
            double A = 1.0;

            double dx = x2 - x1;
            double dy = y2 - y1;
            double L = Math.Sqrt(dx * dx + dy * dy);

            double expected = E * A / L;

            var element = new TrussElement2D(NodeAt(x1, y1), NodeAt(x2, y2), Mat(E), Sec(A));

            // Act
            var k = element.LocalStiffnessMatrix;

            // Assert
            Assert.Equal(expected, k[0, 0]);
            Assert.Equal(-expected, k[0, 2]);
            Assert.Equal(-expected, k[2, 0]);
            Assert.Equal(expected, k[2, 2]);
            Assert.Equal(0.0, k[1, 1]);
            Assert.Equal(0.0, k[3, 3]);
        }
        #endregion

        #region GlobalStiffness
        [Fact]
        public void GlobalStiffness_MatrixIs4x4()
        {
            var element = new TrussElement2D(NodeAt(0, 0), NodeAt(1, 0), Mat(1), Sec(1));

            var k = element.GlobalStiffnessMatrix;

            Assert.Equal(4, k.RowCount);
            Assert.Equal(4, k.ColumnCount);
        }

        [Fact]
        public void GlobalStiffness_IsSymmetric()
        {
            var element = new TrussElement2D(NodeAt(0, 0), NodeAt(3, 4), Mat(1), Sec(1));

            var k = element.GlobalStiffnessMatrix;

            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    Assert.Equal(k[i, j], k[j, i], precision: 10);
        }

        [Fact]
        public void GlobalStiffness_EachRowSumsToZero()
        {
            var element = new TrussElement2D(NodeAt(0, 0), NodeAt(3, 4), Mat(1), Sec(1));
            var k = element.GlobalStiffnessMatrix;

            for (int i = 0; i < 4; i++)
            {
                double rowSum = k.Row(i).Sum();
                Assert.Equal(0.0, rowSum, precision: 10);
            }
        }

        [Fact]
        public void GlobalStiffness_HorizontalElement_EqualsLocalStiffness()
        {
            var element = new TrussElement2D(NodeAt(0, 0), NodeAt(5, 0), Mat(1), Sec(1));

            var kLocal = element.LocalStiffnessMatrix;
            var kGlobal = element.GlobalStiffnessMatrix;

            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    Assert.Equal(kLocal[i, j], kGlobal[i, j]);
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

            var element = new TrussElement2D(NodeAt(x1, y1), NodeAt(x2, y2), Mat(E), Sec(A));

            // Act
            var k = element.GlobalStiffnessMatrix;

            // Assert
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    Assert.Equal(expected[i, j], k[i, j], precision: 10);
        }

        [Fact]
        public void GlobalStiffness_ReversedStartEnd_ProducesIdenticalMatrix()
        {

            var a = NodeAt(1, 2);
            var b = NodeAt(4, 6);

            var forward = new TrussElement2D(a, b, Mat(1), Sec(1));
            var reversed = new TrussElement2D(b, a, Mat(1), Sec(1));

            var kF = forward.GlobalStiffnessMatrix;
            var kR = reversed.GlobalStiffnessMatrix;

            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    Assert.Equal(kF[i, j], kR[i, j]);
        }
        #endregion
    }
}
