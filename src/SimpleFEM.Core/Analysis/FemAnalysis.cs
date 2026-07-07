using SimpleFEM.Core.Domain;
using SimpleFEM.Core.Exceptions;
using SimpleFEM.Core.PostProcessing;
using SimpleFEM.Core.Preprocessing;
using SimpleFEM.Core.Results;
using SimpleFEM.Core.Solver;
using SimpleFEM.Core.Validation;
using SimpleFEM.Core.Validation.Model;
using SimpleFEM.Core.Validation.Result;

namespace SimpleFEM.Core.Analysis;

public class FemAnalysis
{
    private readonly ModelValidator modelValidator;
    private readonly ResultValidator resultValidator;
    private readonly Assembler assembler;
    private readonly PenaltyMethodBoundaryConditionApplier bcApplier;
    private readonly PostProcessor postProcessor;
    private readonly ILinearSolver solver;

    /// <summary>Creates a FEM analyzer with the default options.</summary>
    public FemAnalysis() :this(new FemAnalysisOptions()) { }
    
    /// <summary>Creates a FEM analyzer with the provided options.</summary>
    public FemAnalysis(FemAnalysisOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        modelValidator = new ModelValidator(DefaultValidationRules.Model(options));
        resultValidator = new ResultValidator(DefaultValidationRules.Result(options));
        assembler = new Assembler();
        bcApplier = new PenaltyMethodBoundaryConditionApplier(options.RigidSupportStiffness);
        postProcessor = new PostProcessor();
        solver = new DenseLinearSolver();
    }

    public ValidationResult<ModelValidationErrorCode> Validate(FemModel model) => modelValidator.Validate(model);

    public AnalysisResult Run(FemModel model)
    {
        EnsureValid(model);

        var dofMap = new GlobalDofIndexMap(model.Nodes, model.Elements);
        var restrainedDofs = model.GetRestrainedDofs().ToList();

        var assembledSystem = assembler.Assemble(model, dofMap);
        var constrainedSystem = bcApplier.ApplyBCs(assembledSystem, dofMap, restrainedDofs);

        var u = solver.Solve(constrainedSystem.K, constrainedSystem.F);

        return postProcessor.Recover(model, dofMap, assembledSystem, u);
    }

    private void EnsureValid(FemModel model)
    {
        var validation = modelValidator.Validate(model);
        if (!validation.IsValid)
            throw new InvalidStructureException(
                string.Join("; ", validation.Errors.Select(e => e.Message)));
    }
}