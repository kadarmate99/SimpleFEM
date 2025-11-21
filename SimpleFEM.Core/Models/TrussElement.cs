namespace SimpleFEM.Core.Models
{
    public class TrussElement
    {
        public int Id { get; set; }

        /// <summary>
        /// The line representing the geometry of the element.
        /// </summary>
        public Line Line { get; set; }

        /// <summary>
        /// The cross section of the element.
        /// </summary>
        public CrossSection CrossSection { get; set; }

        /// <summary>
        /// The material of the element.
        /// </summary>
        public Material Material { get; set; }
    }


}
