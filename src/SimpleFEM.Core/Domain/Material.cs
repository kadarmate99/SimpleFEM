namespace SimpleFEM.Core.Domain
{
    public class Material
    {
        public Material(int id, string name, double youngsModulus)
        {
            ArgumentException.ThrowIfNullOrEmpty(name);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(youngsModulus);

            Id = id;
            E = youngsModulus;
            Name = name;
        }

        public int Id { get; }

        public string Name { get; }

        /// <summary>
        /// Young's modulus in Pascal (N/m^2)
        /// </summary>
        public double E { get; }
    }
}