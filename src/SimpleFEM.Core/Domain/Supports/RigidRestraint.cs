namespace SimpleFEM.Core.Domain.Supports
{
    /// <summary>
    /// Rigid restraint on a DOF. The DOF is fixed, dosent have a finite stifness value.
    /// </summary>
    internal sealed record RigidRestraint : Restraint;
}


