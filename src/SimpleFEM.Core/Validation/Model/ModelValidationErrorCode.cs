namespace SimpleFEM.Core.Validation.Model;

/// <summary>
/// Category of a model validation error.
/// </summary>
public enum ModelValidationErrorCode
{
    /// <summary>
    /// Two or more entities of the same kind share an id.
    /// </summary>
    DuplicateIds,

    /// <summary>
    /// An entity references an id that does not exist in the model.
    /// </summary>
    UnknownReference,

    /// <summary>
    /// The model lacks entities required to run an analysis.
    /// </summary>
    EmptyModel,

    /// <summary>
    /// Two nodes are closer than the allowed minimum distance.
    /// </summary>
    CoincidentNodes,

    /// <summary>
    /// An entity is assigned to a node more than once where only one is allowed.
    /// </summary>
    DuplicateNodeAssignment,

    /// <summary>
    /// A loaded DOF receives no stiffness from any element or support.
    /// </summary>
    UnsupportedDof,
}
