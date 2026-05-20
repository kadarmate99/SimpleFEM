using MathNet.Numerics.LinearAlgebra;
using SimpleFEM.Domain.Entities;

namespace SimpleFEM.Core.Elements
{
    public class TrussElement2D
    {
        private Matrix<double>? _localStiffnessMatrix;
        private Matrix<double>? _globalStiffnessMatrix;

        public Node Start { get; }
        public Node End { get; }
        public Material Material { get; }
        public Section Section { get; }

        /// <summary>Unit direction vector from Start to End in global coordinates.</summary>
        public Vector<double> Direction { get; }

        /// <summary>Element length in metres (m).</summary>
        public double L { get; }

        /// <summary>Axial rigidity in Newtons (N).</summary>
        public double EA { get; }


        public TrussElement2D(Node start, Node end, Material material, Section section)
        {
            ArgumentNullException.ThrowIfNull(start);
            ArgumentNullException.ThrowIfNull(end);
            ArgumentNullException.ThrowIfNull(material);
            ArgumentNullException.ThrowIfNull(section);

            double dx = end.Position.X - start.Position.X;
            double dy = end.Position.Y - start.Position.Y;

            double length = Math.Sqrt(dx * dx + dy * dy);

            if (length == 0.0)
                throw new ArgumentException(
                    "Start and end nodes must be different. Element length = 0.",
                    nameof(end));

            Start = start;
            End = end;
            Material = material;
            Section = section;
            L = length;
            EA = material.E * section.A;
            Direction = Vector<double>.Build.DenseOfArray(
            [
                dx / length,
                dy / length
            ]);
        }

        /// <summary>4×4 element stiffness matrix in local coordinates.</summary>
        public Matrix<double> LocalStiffnessMatrix
        {
            get
            {
                _localStiffnessMatrix ??= ComputeLocalStiffnessMatrix();
                return _localStiffnessMatrix;
            }
        }

        /// <summary>4×4 element stiffness matrix in global coordinates.</summary>
        public Matrix<double> GlobalStiffnessMatrix
        {
            get
            {
                _globalStiffnessMatrix ??= ComputeGlobalStiffnessMatrix();
                return _globalStiffnessMatrix;
            }
        }

        private Matrix<double> ComputeLocalStiffnessMatrix()
        {
            double k = EA / L;

            return Matrix<double>.Build.DenseOfArray(new double[,]
            {
                {  k, 0, -k, 0 },
                {  0, 0,  0, 0 },
                { -k, 0,  k, 0 },
                {  0, 0,  0, 0 }
            });
        }

        private Matrix<double> ComputeGlobalStiffnessMatrix()
        {
            var c = Direction[0];
            var s = Direction[1];

            var T = Matrix<double>.Build.DenseOfArray(new double[,]
            {
                { c, s, 0, 0 },
                { -s, c, 0, 0 },
                { 0, 0, c, s },
                { 0, 0, -s, c }
            });

            return T.Transpose() * LocalStiffnessMatrix * T;
        }
    }
}
