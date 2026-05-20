namespace SimpleFEM.Domain.Entities
{
    public class Material
    {
        public Material(Guid id, string name, double e)
        {
            if (id == Guid.Empty) throw new ArgumentException("Id cannot be empty.", nameof(id));
            ArgumentException.ThrowIfNullOrEmpty(name);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(e);

            Id = id;
            E = e;
            Name = name;
        }

        public Guid Id { get; }

        public string Name { get; }

        /// <summary>
        /// Young's modulus in Pascal (N/m^2)
        /// </summary>
        public double E { get; }
    }
}
