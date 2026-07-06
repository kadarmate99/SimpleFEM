using MathNet.Numerics.LinearAlgebra;
using SimpleFEM.Core.Domain;
using SimpleFEM.Core.Results;

namespace SimpleFEM.Core.Elements;

/// <summary>
/// Base type for all finite elements.
/// </summary>
public abstract class Element
{
    private protected Element(int id, IReadOnlyList<int> nodeIds, int materialId, int sectionId)
    {
        Id = id;
        NodeIds = nodeIds;
        MaterialId = materialId;
        SectionId = sectionId;
    }

    public int Id { get; }

    /// <summary>Ids of the connected nodes, in element-local order.</summary>
    public IReadOnlyList<int> NodeIds { get; }

    public int MaterialId { get; }

    public int SectionId { get; }

    internal abstract IReadOnlyList<Dof> GlobalDofs { get; }

    /// <summary>
    /// Element stiffness matrix in global coordinates. Rows/columns follow
    /// <see cref="GlobalDofs"/> order.
    /// </summary>
    internal abstract Matrix<double> ComputeGlobalStiffnessMatrix(ElementContext context);

    /// <summary>
    /// Recovers internal forces from the element's global displacement
    /// vector (ordered like <see cref="GlobalDofs"/>).
    /// </summary>
    internal abstract ElementInternalForceResult ComputeInternalForces(
        ElementContext context, Vector<double> elementDisplacementsGlobal);
}
