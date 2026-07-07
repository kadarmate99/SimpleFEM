namespace SimpleFEM.Core.Analysis;

/// <summary>
/// Settings for <see cref="FemAnalysis"/>.
/// </summary>
public sealed class FemAnalysisOptions
{
    /// <summary>
    /// Penalty stiffness added to the global stiffness diagonal for
    /// rigidly restrained DOFs, in N/m (translation) or Nm/rad (rotation).
    /// </summary>
    public double RigidSupportStiffness { get; init; } = 1e13;

    /// <summary>
    /// Two nodes closer than this in m are considered coincident.
    /// </summary>
    public double MinNodeDistance { get; init; } = 1e-9;

}
