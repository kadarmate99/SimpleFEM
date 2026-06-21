using MathNet.Numerics.LinearAlgebra;

namespace SimpleFEM.Core.Solver;

internal sealed class DenseLinearSolver : ILinearSolver
{
    public Vector<double> Solve(Matrix<double> k, Vector<double> f) => k.Solve(f);
}
