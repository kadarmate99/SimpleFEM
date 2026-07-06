using SimpleFEM.Core.Domain.Supports;
using SimpleFEM.Core.Elements;
using SimpleFEM.Core.Loads;

namespace SimpleFEM.Core.Domain;


public sealed class FemModel
{
    public IReadOnlyList<Node> Nodes { get; }
    public IReadOnlyList<Material> Materials { get; }
    public IReadOnlyList<CrossSection> Sections { get; }
    public IReadOnlyList<ILineElement> Elements { get; }
    public IReadOnlyList<NodalLoad> Loads { get; }
    public IReadOnlyList<Support> Supports { get; }

    private readonly Dictionary<int, Node> _nodeById;
    private readonly Dictionary<int, Material> _materialById;
    private readonly Dictionary<int, CrossSection> _sectionById;

    public FemModel(
        IReadOnlyList<Node> nodes,
        IReadOnlyList<Material> materials,
        IReadOnlyList<CrossSection> sections,
        IReadOnlyList<ILineElement> elements,
        IReadOnlyList<NodalLoad> loads,
        IReadOnlyList<Support> supports)
    {
        Nodes = nodes;
        Materials = materials;
        Sections = sections;
        Elements = elements;
        Loads = loads;
        Supports = supports;

        _nodeById = Nodes.ToDictionary(n => n.Id);
        _materialById = Materials.ToDictionary(m => m.Id);
        _sectionById = Sections.ToDictionary(s => s.Id);
    }

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