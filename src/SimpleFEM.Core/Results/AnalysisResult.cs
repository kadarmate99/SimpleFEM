namespace SimpleFEM.Core.Results;

public record AnalysisResult(
    IReadOnlyList<NodalDisplacementResult> NodalDisplacements,
    IReadOnlyList<ReactionResult> Reactions,
    IReadOnlyList<ElementInternalForceResult> ElementInternalForces);
