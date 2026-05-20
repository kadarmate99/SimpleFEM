namespace SimpleFEM.Domain.Entities
{
    public class TrussElement
    {
        public TrussElement(Guid id, Guid startNodeId, Guid endNodeId, Guid materialId, Guid sectionId)
        {
            if (id == Guid.Empty) 
                throw new ArgumentException("Id cannot be empty.", nameof(id));
            if (startNodeId == Guid.Empty) 
                throw new ArgumentException("Id cannot be empty.", nameof(startNodeId));
            if (endNodeId == Guid.Empty) 
                throw new ArgumentException("Id cannot be empty.", nameof(endNodeId));
            if (materialId == Guid.Empty) 
                throw new ArgumentException("Id cannot be empty.", nameof(materialId));
            if (sectionId == Guid.Empty) 
                throw new ArgumentException("Id cannot be empty.", nameof(sectionId));
            if (startNodeId == endNodeId) 
                throw new ArgumentException("Start and end nodes must differ.");

            Id = id;
            StartNodeId = startNodeId;
            EndNodeId = endNodeId;
            MaterialId = materialId;
            SectionId = sectionId;
        }

        public Guid Id { get; }
        public Guid StartNodeId { get; }
        public Guid EndNodeId { get; }
        public Guid MaterialId { get; }
        public Guid SectionId { get; }
    }
}
