namespace SimpleFEM.Core.Models
{
    public class Load : IEntity
    {
        public int Id { get; set; }

        /// <summary>
        /// The node where the lode is applied
        /// </summary>
        public Node Node { get; set; }

        /// <summary>
        /// The intensity of the load in the X direction in kN. Can be negative.
        /// </summary>
        public double ForceX { get; set; }

        /// <summary>
        /// The intensity of the load in the Z direction in kN. Can be negative.
        /// </summary>
        public double ForceZ { get; set; }
    }


}
