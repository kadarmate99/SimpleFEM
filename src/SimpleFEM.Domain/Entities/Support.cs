namespace SimpleFEM.Domain.Entities
{
    public class Support
    {
        public Support(Guid id, Guid nodeId, bool isUxFixed, bool isUyFixed)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Id cannot be empty.", nameof(id));
            if (nodeId == Guid.Empty)
                throw new ArgumentException("Id cannot be empty.", nameof(nodeId));
            if (isUxFixed == false && isUyFixed == false)
                throw new ArgumentException("A support can't be free in both directions");

            Id = id;
            NodeId = nodeId;
            IsUxFixed = isUxFixed;
            IsUyFixed = isUyFixed;
        }

        public Guid Id { get; }
        public Guid NodeId { get; }

        /// <summary>
        /// Does this support block displacement in the X direction
        /// </summary>
        public bool IsUxFixed { get; }

        /// <summary>
        /// Does this support block displacement in the Y direction
        /// </summary>
        public bool IsUyFixed { get; }
    }
}
