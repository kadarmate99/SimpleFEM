using SimpleFEM.Core.Domain.Supports;
using SimpleFEM.Core.Elements;
using SimpleFEM.Core.Loads;

namespace SimpleFEM.Core.Domain;

public record FemModel(
    IReadOnlyList<Node> Nodes,
    IReadOnlyList<Material> Materials,
    IReadOnlyList<CrossSection> Sections,
    IReadOnlyList<ILineElement> Elements,
    IReadOnlyList<NodalLoad> Loads,
    IReadOnlyList<Support> Supports
)
{
    private readonly Dictionary<int, Node> _nodeById = Nodes.ToDictionary(n => n.Id);
    private readonly Dictionary<int, Material> _materialById = Materials.ToDictionary(m => m.Id);
    private readonly Dictionary<int, CrossSection> _sectionById = Sections.ToDictionary(s => s.Id);

    internal Node GetNode(int id) =>
        _nodeById.TryGetValue(id, out var n)
            ? n
            : throw new KeyNotFoundException($"Node {id} not found.");

    internal Material GetMaterial(int id) =>
        _materialById.TryGetValue(id, out var m)
            ? m
            : throw new KeyNotFoundException($"Material {id} not found.");

    internal CrossSection GetSection(int id) =>
        _sectionById.TryGetValue(id, out var s)
            ? s
            : throw new KeyNotFoundException($"Section {id} not found.");

    internal IEnumerable<RestrainedDof> GetRestrainedDofs()
        => Supports.SelectMany(s => s.GetRestrainedDofs());
}