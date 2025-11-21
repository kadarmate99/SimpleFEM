namespace SimpleFEM.Core.Models
{
    public class Material
    {
        public int Id { get; set; }

        /// <summary>
        /// Young modulus of the material in Kilonewton per Square meter.
        /// </summary>
        public double EModulus { get; set; }
    }


}
