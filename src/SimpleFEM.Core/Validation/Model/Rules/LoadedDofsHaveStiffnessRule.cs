using SimpleFEM.Core.Domain;
using System.Data;

namespace SimpleFEM.Core.Validation.Model.Rules;

internal sealed class LoadedDofsHaveStiffnessRule : IModelValidationRule
{
    public IEnumerable<ValidationError<ModelValidationErrorCode>> Validate(FemModel model)
    {
        // Every DOF that some element contributes stiffness to.
        var supportedDofs = model.Elements
            .SelectMany(e => e.GlobalDofs)
            .Concat(model.Supports.SelectMany(s => s.GetRestrainedDofs().Select(r => r.Dof)))
            .ToHashSet();

        foreach (var load in model.Loads)
            foreach (var dofValue in load.GetLoadsOnDofs())
                if (!supportedDofs.Contains(dofValue.Dof))
                    yield return new(
                        ModelValidationErrorCode.UnsupportedDof,
                        $"Node {load.NodeId} has a load on {dofValue.Dof.Type} " +
                        $"but no element provides stiffness there; the system is singular.",
                        load);
    }
}
