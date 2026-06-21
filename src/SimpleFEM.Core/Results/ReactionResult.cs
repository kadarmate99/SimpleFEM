namespace SimpleFEM.Core.Results;

public record ReactionResult(
    int NodeId,
    double Rx,
    double Ry,
    double Mz);
