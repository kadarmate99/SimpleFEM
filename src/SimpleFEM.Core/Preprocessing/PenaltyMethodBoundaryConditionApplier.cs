using MathNet.Numerics.LinearAlgebra;
using SimpleFEM.Core.Domain.Supports;

namespace SimpleFEM.Core.Preprocessing
{
    internal class PenaltyMethodBoundaryConditionApplier
    {
        private readonly double _rigidSupportStiffness;

        internal PenaltyMethodBoundaryConditionApplier(double rigidSupportStiffness = 1e10)
        {
            _rigidSupportStiffness = rigidSupportStiffness;
        }

        internal (Matrix<double> K, Vector<double> F) ApplyBCs(
            GlobalSystem system,
            IReadOnlyList<RestrainedDof> restrainedDofs)
        {
            var k = system.K.Clone();
            var f = system.F.Clone(); // will be relevant when implementing prescribed nodal displacment loads.

            foreach (var restrain in restrainedDofs)
            {
                if (!system.DofMap.TryGetGlobalIndex(restrain.Dof, out var dofId))
                    continue; // skipp restrain if ifs ont on an active dof

                double stiffness = restrain.Restraint switch
                {
                    RigidRestraint => _rigidSupportStiffness,
                    ElasticRestraint e => e.Stiffness,
                    _ => throw new NotSupportedException(
                        $"Unknown restraint type {restrain.Restraint.GetType()}")
                };

                k[dofId, dofId] += stiffness;
            }

            return (k, f);
        }
    }
}
