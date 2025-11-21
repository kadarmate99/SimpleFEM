namespace SimpleFEM.Core.Models
{
    public class Node
    {
        public int Id { get; set; }

        /// <summary>
        /// X-coordinate of the node in meters.
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// Z-coordinate of the node in meters.
        /// </summary>
        public double Z { get; set; }
    }
}
