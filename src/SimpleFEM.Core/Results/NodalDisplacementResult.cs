namespace SimpleFEM.Core.Results;

public record NodalDisplacementResult(
    int NodeId, 
    double Ux,
    double Uy,
    double Rz);
