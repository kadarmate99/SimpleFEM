namespace SimpleFEM.Domain.Entities
{
    public class Section
    {
        public Section(Guid id, string name, double a)
        {
            if (id == Guid.Empty) throw new ArgumentException("Id cannot be empty.", nameof(id));
            ArgumentException.ThrowIfNullOrEmpty(name);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(a);

            Id = id;
            Name = name;
            A = a;
        }

        public Guid Id { get; }
        public string Name { get; }

        /// <summary>
        /// Cross-sectional area in Square Meter (m²)
        /// </summary>
        public double A { get; }
    }
}
