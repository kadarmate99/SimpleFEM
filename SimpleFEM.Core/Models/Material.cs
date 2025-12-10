using SimpleFEM.Core.Interfaces;

namespace SimpleFEM.Core.Models
{
    public class Material : IEntity
    {
        public int Id { get; set; }

        /// <summary>
        /// Young modulus of the material in Kilonewton per Square meter.
        /// </summary>
        public double EModulus { get; set; }
    }


}
