using SimpleFEM.Core.Domain.Supports;

namespace SimpleFEM.Core.Preprocessing;

internal class PenaltyMethodBoundaryConditionApplier
{
    private readonly double _rigidSupportStiffness;

    internal PenaltyMethodBoundaryConditionApplier(double rigidSupportStiffness = 1e13)
    {
        _rigidSupportStiffness = rigidSupportStiffness;
    }

    internal GlobalSystem ApplyBCs(
        GlobalSystem system,
        GlobalDofIndexMap dofMap,
        IReadOnlyList<RestrainedDof> restrainedDofs)
    {
        var k = system.K.Clone();
        var f = system.F.Clone(); // will be relevant when implementing prescribed nodal displacement loads.

        foreach (var restrain in restrainedDofs)
        {
            if (!dofMap.TryGetGlobalIndex(restrain.Dof, out var dofId))
                continue; // skip restraint if ifs not on an active dof

            double stiffness = restrain.Restraint.Stiffness ?? _rigidSupportStiffness;

            k[dofId, dofId] += stiffness;
        }

        return new GlobalSystem(k, f);
    }
}
