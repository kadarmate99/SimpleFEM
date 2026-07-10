using SimpleFEM.Core.Domain;
using SimpleFEM.Core.Domain.Supports;
using SimpleFEM.Core.Elements;
using SimpleFEM.Core.Loads;

namespace SimpleFEM.Core.Tests.Validation;

internal static class ModelBuilder
{

    /// <summary>
    /// If no params passed builds a fully valid model using default valid values. 
    /// Pass params to override defaults.
    /// </summary>
    public static FemModel Build(
        IReadOnlyList<Node>? nodes = null,
        IReadOnlyList<Material>? materials = null,
        IReadOnlyList<CrossSection>? sections = null,
        IReadOnlyList<Element>? elements = null,
        IReadOnlyList<NodalLoad>? loads = null,
        IReadOnlyList<Support>? supports = null)
        => new(
            nodes ?? [new Node(0, 0.0, 0.0), new Node(1, 1.0, 0.0)],
            materials ?? [new Material(0, "S235", 210e9)],
            sections ?? [new CrossSection(0, "A", 1e-3)],
            elements ?? [new TrussElement2D(0, 0, 1, 0, 0)],
            loads ?? [new NodalLoad(1, 10.0, 0.0, 0.0)],
            supports ??
            [
                new Support(0, Restraint.Rigid(), Restraint.Rigid()),
                new Support(1, uy: Restraint.Elastic(1e4)),
            ]);
}
