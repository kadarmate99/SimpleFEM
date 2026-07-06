namespace SimpleFEM.Core.Domain.Supports;

/// <summary>
/// Restraint condition on a DOF direction
/// </summary>
public sealed record Restraint
{
    private static readonly double? RigidStiffness = null;
    
    private Restraint(double? stiffness) => Stiffness = stiffness;

    /// <summary>Spring stiffness in N/m for displacement, or Nm/rad for rotation; 
    /// null when rigid.</summary>
    public double? Stiffness { get; }

    public bool IsRigid => Stiffness is null;

    public static Restraint Rigid() => new(RigidStiffness);

    /// <summary>
    /// Returns a linear elastic spring restraint with the provided spring stiffness 
    /// </summary>
    /// <param name="stiffness">
    /// Spring stiffness in N/m for displacement, or Nm/rad for rotation
    /// </param>
    public static Restraint Elastic(double stiffness)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(stiffness);
        return new Restraint(stiffness);
    }
}


