using SimpleFEM.Domain.ValueObjects;

namespace SimpleFEM.Domain.Entities
{
    public record NodalLoad
    {
        public NodalLoad(Guid nodeId, Force2D force)
        {
            if (nodeId == Guid.Empty)
                throw new ArgumentException("Id cannot be empty.", nameof(nodeId));
            ArgumentNullException.ThrowIfNull(force);

            NodeId = nodeId;
            Force = force;
        }

        public Guid NodeId { get; }
        public Force2D Force { get; }
    }
}
