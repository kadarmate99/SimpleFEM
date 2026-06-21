namespace SimpleFEM.Core.Domain.Supports;

internal readonly record struct RestrainedDof(Dof Dof, Restraint Restraint);
