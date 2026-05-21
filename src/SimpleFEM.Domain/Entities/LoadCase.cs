using SimpleFEM.Domain.ValueObjects;

namespace SimpleFEM.Domain.Entities
{
    public class LoadCase
    {
        public LoadCase(Guid id, string name, IReadOnlyList<NodalLoad> nodalLoads)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Id cannot be empty.", nameof(id));
            ArgumentException.ThrowIfNullOrEmpty(name);
            ArgumentNullException.ThrowIfNull(nodalLoads);

            Id = id;
            Name = name;
            NodalLoads = nodalLoads;
        }

        public Guid Id { get; }
        public string Name { get; }
        public IReadOnlyList<NodalLoad> NodalLoads { get; }
    }
}
