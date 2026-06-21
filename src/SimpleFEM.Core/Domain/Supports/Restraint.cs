namespace SimpleFEM.Core.Domain.Supports;

/// <summary>
/// Restraint condition on a DOF direction
/// </summary>
public abstract record Restraint
{
    private protected Restraint() { }

    public static Restraint Rigid() => new RigidRestraint();

    /// <summary>
    /// Returns a linear elastic spring restraint with the provided spring stifness 
    /// </summary>
    /// <param name="stiffness">
    /// Spring stifness in N/m for displacement, or Nm/rad for rotation
    /// </param>
    public static Restraint Elastic(double stiffness)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(stiffness);
        return new ElasticRestraint(stiffness);
    }
}


