namespace SimpleFEM.Core.Results;

public record ElementInternalForceResult(
    int ElementId,
    double N,
    double V,
    double M);
