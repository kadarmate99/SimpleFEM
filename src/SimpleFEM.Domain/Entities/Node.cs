using SimpleFEM.Domain.ValueObjects;

namespace SimpleFEM.Domain.Entities
{
    public class Node
    {
        public Node(Guid id, Coordinate2D position)
        {
            if (id == Guid.Empty) throw new ArgumentException("Id cannot be empty.", nameof(id));
            ArgumentNullException.ThrowIfNull(position);

            Id = id;
            Position = position;
        }

        public Guid Id { get; }

        public Coordinate2D Position { get; }
    }
}
