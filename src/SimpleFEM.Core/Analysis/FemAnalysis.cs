using SimpleFEM.Core.Domain;
using SimpleFEM.Core.Domain.Supports;
using SimpleFEM.Core.Exceptions;
using SimpleFEM.Core.PostProcessing;
using SimpleFEM.Core.Preprocessing;
using SimpleFEM.Core.Results;
using SimpleFEM.Core.Solver;

namespace SimpleFEM.Core.Analysis;

public class FemAnalysis
{
    private readonly ModelValidator _validator = new();
    private readonly Assembler _assembler = new();
    private readonly PenaltyMethodBoundaryConditionApplier _bcApplier = new();
    private readonly PostProcessor _postProcessor = new();
    private readonly ILinearSolver _solver = new DenseLinearSolver();

    public ModelValidationResult Validate(FemModel model) => _validator.Validate(model);

    public AnalysisResult Run(FemModel model)
    {
        EnsureValid(model);

        var dofMap = new GlobalDofIndexMap(model.Nodes, model.Elements);
        var restrainedDofs = model.GetRestrainedDofs().ToList();

        var assembledSystem = _assembler.Assemble(model, dofMap);
        var constrainedSystem = _bcApplier.ApplyBCs(assembledSystem, dofMap, restrainedDofs);

        var u = _solver.Solve(constrainedSystem.K, constrainedSystem.F);

        return _postProcessor.Recover(model, dofMap, assembledSystem, u);
    }

    private void EnsureValid(FemModel model)
    {
        var validation = _validator.Validate(model);
        if (!validation.IsValid)
            throw new InvalidStructureException(
                string.Join("; ", validation.Errors.Select(e => e.Message)));
    }
}
