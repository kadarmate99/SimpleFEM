namespace SimpleFEM.Domain.Entities
{
    public class Structure
    {
        public Structure(
            Guid id,
            IReadOnlyList<Node> nodes,
            IReadOnlyList<TrussElement> elements,
            IReadOnlyList<Material> materials,
            IReadOnlyList<Section> sections,
            IReadOnlyList<Support> supports,
            IReadOnlyList<LoadCase> loadCases)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Id cannot be empty.", nameof(id));
            ArgumentNullException.ThrowIfNull(nodes);
            ArgumentNullException.ThrowIfNull(elements);
            ArgumentNullException.ThrowIfNull(materials);
            ArgumentNullException.ThrowIfNull(sections);
            ArgumentNullException.ThrowIfNull(supports);
            ArgumentNullException.ThrowIfNull(loadCases);

            Id = id;
            Nodes = nodes;
            Elements = elements;
            Materials = materials;
            Sections = sections;
            Supports = supports;
            LoadCases = loadCases;
        }


        public Guid Id { get; }
        public IReadOnlyList<Node> Nodes { get; }
        public IReadOnlyList<TrussElement> Elements { get; }
        public IReadOnlyList<Material> Materials { get; }
        public IReadOnlyList<Section> Sections { get; }
        public IReadOnlyList<Support> Supports { get; }
        public IReadOnlyList<LoadCase> LoadCases { get; }
        public int NumDof => Nodes.Count * 2;
    }
}
