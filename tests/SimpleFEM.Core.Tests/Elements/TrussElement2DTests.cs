using MathNet.Numerics.LinearAlgebra;
using SimpleFEM.Core.Domain;
using SimpleFEM.Core.Elements;
using static System.Collections.Specialized.BitVector32;

namespace SimpleFEM.Core.Tests.Elements;

public class TrussElement2DTests
{
    #region ComputeGlobalStiffnessMatrix
    [Fact]
    public void ComputeGlobalStiffnessMatrix_MatrixIs4x4()
    {
        // Arrange
        var nodeI = new Node(0, 0.0, 0.0);
        var nodeJ = new Node(1, 2.0, 0.0);
        var material = new Material(0, "Steel", 200e9);
        var section = new CrossSection(0, "A", 0.01);

        var element = new TrussElement2D(0, nodeI.Id, nodeJ.Id, material.Id, section.Id);
        var context = new ElementContext([nodeI, nodeJ], material, section);

        // Act
        var kg = element.ComputeGlobalStiffnessMatrix(context);

        // Assert 
        Assert.Equal(4, kg.RowCount);
        Assert.Equal(4, kg.ColumnCount);
    }

    [Fact]
    public void ComputeGlobalStiffnessMatrix_IsSymmetric()
    {
        // Arrange
        var nodeI = new Node(0, 0.0, 0.0);
        var nodeJ = new Node(1, 2.0, 0.0);
        var material = new Material(0, "Steel", 200e9);
        var section = new CrossSection(0, "A", 0.01);

        var element = new TrussElement2D(0, nodeI.Id, nodeJ.Id, material.Id, section.Id);
        var context = new ElementContext([nodeI, nodeJ], material, section);

        // Act
        var kg = element.ComputeGlobalStiffnessMatrix(context);

        for (int i = 0; i < 4; i++)
            for (int j = 0; j < 4; j++)
                Assert.Equal(kg[i, j], kg[j, i], Tolerances.Tol);
    }

    [Fact]
    public void ComputeGlobalStiffnessMatrix_EachRowSumsToZero()
    {
        // Arrange
        var nodeI = new Node(0, 0.0, 0.0);
        var nodeJ = new Node(1, 2.0, 0.0);
        var material = new Material(0, "Steel", 200e9);
        var section = new CrossSection(0, "A", 0.01);

        var element = new TrussElement2D(0, nodeI.Id, nodeJ.Id, material.Id, section.Id);
        var context = new ElementContext([nodeI, nodeJ], material, section);

        // Act
        var kg = element.ComputeGlobalStiffnessMatrix(context);

        for (int i = 0; i < 4; i++)
        {
            double rowSum = kg.Row(i).Sum();
            Assert.Equal(0.0, rowSum, Tolerances.Tol);
        }
    }

    [Theory]
    [InlineData(0, 0, 3, 0)]
    [InlineData(0, 0, 0, 4)]
    [InlineData(0, 0, 3, 4)]
    [InlineData(-1, -1, 2, 3)]
    public void ComputeGlobalStiffnessMatrix_MatchesHandCalculation(double x1, double y1, double x2, double y2)
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
        var context = new ElementContext([nodeI, nodeJ], material, section);

        // Act
        var kg = element.ComputeGlobalStiffnessMatrix(context);

        // Assert
        for (int i = 0; i < 4; i++)
            for (int j = 0; j < 4; j++)
                Assert.Equal(expected[i, j], kg[i, j], Tolerances.Tol);
    }

    [Fact]
    public void ComputeGlobalStiffnessMatrix_ReversedStartEnd_ProducesIdenticalMatrix()
    {
        // Arrange
        var a = new Node(0, 0.0, 0.0);
        var b = new Node(1, 2.0, 0.0);
        var material = new Material(0, "Steel", 200e9);
        var section = new CrossSection(0, "A", 0.01);

        var forward = new TrussElement2D(0, a.Id, b.Id, material.Id, section.Id);
        var forwardContext = new ElementContext([a, b], material, section);

        var reversed = new TrussElement2D(0, b.Id, a.Id, material.Id, section.Id);
        var reversedContext = new ElementContext([b, a], material, section);


        // Act
        var kgF = forward.ComputeGlobalStiffnessMatrix(forwardContext);
        var kgR = reversed.ComputeGlobalStiffnessMatrix(reversedContext);

        // Assert
        for (int i = 0; i < 4; i++)
            for (int j = 0; j < 4; j++)
                Assert.Equal(kgF[i, j], kgR[i, j]);
    }
    #endregion

    #region ComputeInternalForces
    [Theory]
    [InlineData(0.1, 2)]   // tension
    [InlineData(-0.1, -2)]  // compression
    public void ComputeInternalForces_ElongationProducesCorrectSign(double elongation, double expectedN)
    {
        var nodeI = new Node(0, 0, 0);
        var nodeJ = new Node(1, 3, 4);  // L = 5
        var mat = new Material(0, "m", 100); var sec = new CrossSection(0, "a", 1.0); // k = EA/L = 20
        var el = new TrussElement2D(0, 0, 1, 0, 0);
        var context = new ElementContext([nodeI, nodeJ], mat, sec);

        var dx = nodeJ.X - nodeI.X;
        var dy = nodeJ.Y - nodeI.Y;
        var L = Math.Sqrt(dx * dx + dy * dy);
        var c = dx / L;
        var s = dy / L;

        var u = Vector<double>.Build.DenseOfArray([0.0, 0.0, c * elongation, s * elongation]);

        var result = el.ComputeInternalForces(context, u);

        Assert.Equal(expectedN, result.N, Tolerances.Tol);  // N = k*elongation = 20*0.1 = 2
    }

    [Theory]
    [InlineData(0, 0, 3, 4)]
    [InlineData(3, 4, 0, 0)]
    [InlineData(0, 0, -4, 3)]
    [InlineData(1, 2, 5, 6)]
    public void ComputeInternalForces_IndependentOfElementOrientation(double x1, double y1, double x2, double y2)
    {
        var f = 10;

        var mat = new Material(0, "m", 100);
        var sec = new CrossSection(0, "a", 1.0);
        var el = new TrussElement2D(0, 0, 1, 0, 0);

        var nodeI = new Node(0, x1, y1);
        var nodeJ = new Node(1, x2, y2);

        var context = new ElementContext([nodeI, nodeJ], mat, sec);

        var dx = x2 - x1;
        var dy = y2 - y1;
        var L = Math.Sqrt(dx * dx + dy * dy);
        var c = dx / L;
        var s = dy / L;

        // elongation = N/(EA/L) 
        var elongation = f / (mat.E * sec.A / L);

        var u = Vector<double>.Build.DenseOfArray([0.0, 0.0, c * elongation, s * elongation]);

        var result = el.ComputeInternalForces(context, u);

        Assert.Equal(f, result.N, Tolerances.Tol);
        Assert.Equal(0, result.V, Tolerances.Tol);
        Assert.Equal(0, result.M, Tolerances.Tol);
    }


    [Fact]
    public void ComputeInternalForces_NodeOrderReversal_ReversesSign()
    {
        var mat = new Material(0, "m", 100);
        var sec = new CrossSection(0, "a", 1.0);
        var el = new TrussElement2D(0, 0, 1, 0, 0);

        var nodeI = new Node(0, 0, 0);
        var nodeJ = new Node(3, 4, 0);
        var context = new ElementContext([nodeI, nodeJ], mat, sec);
        var contextReversed = new ElementContext([nodeJ, nodeI], mat, sec);

        var u = Vector<double>.Build.DenseOfArray([0.0, 0.0, 0.6, 0.8]);

        var f1 = el.ComputeInternalForces(context, u);
        var f2 = el.ComputeInternalForces(contextReversed, u);

        Assert.Equal(f1.N, -f2.N, 1e-9);
    }

    [Theory]
    [InlineData(0, 0, 3, 4)]
    [InlineData(1, 2, 5, 6)]
    public void ComputeInternalForces_IsZero_ForTransverseAndRigidBodyMotion(
        double x1, double y1, double x2, double y2)
    {
        var mat = new Material(0, "m", 100); var sec = new CrossSection(0, "a", 1.0);
        var el = new TrussElement2D(0, 0, 1, 0, 0);
        var nodeI = new Node(0, x1, y1); var nodeJ = new Node(1, x2, y2);
        var context = new ElementContext([nodeI, nodeJ], mat, sec);
        var (dx, dy) = (x2 - x1, y2 - y1); var L = Math.Sqrt(dx * dx + dy * dy);
        var (c, s) = (dx / L, dy / L);

        // J moved perpendicular to the axis
        var transverse = Vector<double>.Build.DenseOfArray([0, 0, -s * 0.3, c * 0.3]);
        var result1 = el.ComputeInternalForces(context, transverse);
        Assert.Equal(0, result1.N, Tolerances.Tol);

        // brigid body movement
        var rigid = Vector<double>.Build.DenseOfArray([0.7, -0.4, 0.7, -0.4]);
        var result2 = el.ComputeInternalForces(context, rigid);
        Assert.Equal(0, result2.N, Tolerances.Tol);
    }
    #endregion
}
