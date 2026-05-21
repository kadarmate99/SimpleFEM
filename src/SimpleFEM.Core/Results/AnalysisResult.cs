namespace SimpleFEM.Core.Results
{
    public record AnalysisResult(
        Guid StructureId,
        Guid LoadCaseId,
        IReadOnlyList<NodalDisplacementResult> NodalDisplacements,
        IReadOnlyList<ReactionResult> Reactions,
        IReadOnlyList<ElementForceResult> ElementForces);
}
