using SimpleFEM.Core.Domain.Supports;
using SimpleFEM.Core.Elements;
using SimpleFEM.Core.Loads;

namespace SimpleFEM.Core.Domain;


public sealed class FemModel
{
    public IReadOnlyList<Node> Nodes { get; }
    public IReadOnlyList<Material> Materials { get; }
    public IReadOnlyList<CrossSection> Sections { get; }
    public IReadOnlyList<Element> Elements { get; }
    public IReadOnlyList<NodalLoad> Loads { get; }
    public IReadOnlyList<Support> Supports { get; }

    private readonly Dictionary<int, Node> _nodeById;
    private readonly Dictionary<int, Material> _materialById;
    private readonly Dictionary<int, CrossSection> _sectionById;

    public FemModel(
        IReadOnlyList<Node> nodes,
        IReadOnlyList<Material> materials,
        IReadOnlyList<CrossSection> sections,
        IReadOnlyList<Element> elements,
        IReadOnlyList<NodalLoad> loads,
        IReadOnlyList<Support> supports)
    {
        Nodes = nodes;
        Materials = materials;
        Sections = sections;
        Elements = elements;
        Loads = loads;
        Supports = supports;


        // ToDictionary() throws ArgumentException on duplicate ids,
        // duplicate id detection is the responsibility of the validation rules so it can be reported
        _nodeById = BuildLookup(Nodes, n => n.Id);
        _materialById = BuildLookup(Materials, m => m.Id);
        _sectionById = BuildLookup(Sections, s => s.Id);
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

    internal ElementContext Resolve(Element element)
    {
        var nodes = new Node[element.NodeIds.Count];
        for (int i = 0; i < nodes.Length; i++)
            nodes[i] = GetNode(element.NodeIds[i]);

        return new ElementContext(
            nodes,
            GetMaterial(element.MaterialId),
            GetSection(element.SectionId));
    }

    private static Dictionary<int, T> BuildLookup<T>(IReadOnlyList<T> items, Func<T, int> idOf)
    {
        var lookup = new Dictionary<int, T>(items.Count);
        foreach (var item in items)
            lookup[idOf(item)] = item;
        return lookup;
    }
}