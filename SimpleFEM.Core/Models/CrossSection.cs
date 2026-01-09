namespace SimpleFEM.Core.Models
{
    public class CrossSection : IEntity
    {
        public int Id { get; set; }

        /// <summary>
        /// Area of the cross section in square meters.
        /// </summary>
        public double Area { get; set; }
    }


}
