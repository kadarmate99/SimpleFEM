using SimpleFEM.Core.Interfaces;

namespace SimpleFEM.Core.Models
{
    public class Line : IEntity
    {
        public int Id { get; set; }

        /// <summary>
        /// Start node of the line.
        /// </summary>
        public Node INode { get; set; }

        /// <summary>
        /// End node of the line.
        /// </summary>
        public Node JNode { get; set; }
    }


}
