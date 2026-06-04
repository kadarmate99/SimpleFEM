namespace SimpleFEM.Core.Domain.Supports
{
    /// <summary>
    /// A linear elastic spring restraint on a DOF in N/m for displacement, or Nm/rad for rotation.
    /// </summary>
    internal sealed record ElasticRestraint(double Stiffness) : Restraint;
}


