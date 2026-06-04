using MathNet.Numerics.LinearAlgebra;

namespace SimpleFEM.Core.Preprocessing
{
    internal record GlobalSystem(
        GlobalDofIndexMap DofMap,
        Matrix<double> K,
        Vector<double> F);
}
