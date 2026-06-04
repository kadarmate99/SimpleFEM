namespace SimpleFEM.Core.Domain
{
    public class CrossSection
    {
        public CrossSection(int id, string name, double area)
        {
            ArgumentException.ThrowIfNullOrEmpty(name);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(area);

            Id = id;
            Name = name;
            A = area;
        }

        public int Id { get; }
        public string Name { get; }

        /// <summary>
        /// Cross-sectional area in Square Meter (m²)
        /// </summary>
        public double A { get; }
    }
}
