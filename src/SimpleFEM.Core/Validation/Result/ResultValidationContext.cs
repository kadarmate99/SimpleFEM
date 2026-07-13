using MathNet.Numerics.LinearAlgebra;
using SimpleFEM.Core.Analysis;
using SimpleFEM.Core.Domain;
using SimpleFEM.Core.Domain.Supports;
using SimpleFEM.Core.Preprocessing;
using SimpleFEM.Core.Results;

namespace SimpleFEM.Core.Validation.Result;

/// <summary>
/// Input for analysis result verification.
/// </summary>
internal sealed record ResultValidationContext(
    FemModel Model,
    AnalysisResult Result,
    GlobalDofIndexMap DofMap,
    GlobalSystem AssembledSystem,
    GlobalSystem ConstrainedSystem,
    Vector<double> Displacements,  
    IReadOnlyList<RestrainedDof> RestrainedDofs,
    FemAnalysisOptions Options);
