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
    private readonly FemAnalysisOptions options;

    /// <summary>Creates a FEM analyzer with the default options.</summary>
    public FemAnalysis() : this(new FemAnalysisOptions()) { }

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
        this.options = options;
    }

    public ValidationResult<ModelValidationErrorCode> Validate(FemModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        return modelValidator.Validate(model);
    }

    /// <summary>
    /// Runs the analysis. 
    /// Before starting the analysis validates <paramref name="model"/> 
    /// and throws <see cref="InvalidModelException"/> if it is invalid.
    /// </summary>
    /// <exception cref="ArgumentNullException"><paramref name="model"/> is null.</exception>
    /// <exception cref="InvalidModelException">The model failed pre-run validation.</exception>
    public AnalysisOutcome Run(FemModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        var modelValidation = modelValidator.Validate(model);
        if (!modelValidation.IsValid)
            throw new InvalidModelException(modelValidation, model, nameof(model));

        var dofMap = new GlobalDofIndexMap(model.Nodes, model.Elements);
        var restrainedDofs = model.GetRestrainedDofs().ToList();

        var assembledSystem = assembler.Assemble(model, dofMap);
        var constrainedSystem = bcApplier.ApplyBCs(assembledSystem, dofMap, restrainedDofs);

        var u = solver.Solve(constrainedSystem.K, constrainedSystem.F);

        var result = postProcessor.Recover(model, dofMap, assembledSystem, u);

        var resultContext = new ResultValidationContext(
            model,
            result,
            dofMap,
            assembledSystem,
            constrainedSystem,
            u,
            restrainedDofs,
            options);

        var resultValidation = resultValidator.Validate(resultContext);
        return new AnalysisOutcome(result, resultValidation);
    }
}