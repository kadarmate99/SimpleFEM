using MathNet.Numerics.LinearAlgebra;

namespace SimpleFEM.Core.Preprocessing
{
    internal record GlobalSystem(
        Matrix<double> K,
        Vector<double> F);
}
