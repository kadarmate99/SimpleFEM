using MathNet.Numerics.LinearAlgebra;

namespace SimpleFEM.Core.Solver;

internal interface ILinearSolver
{
    /// <summary>
    /// Solves K·u = f
    /// </summary>
    /// <returns>
    /// u
    /// </returns>
    Vector<double> Solve(Matrix<double> k, Vector<double> f);
}
