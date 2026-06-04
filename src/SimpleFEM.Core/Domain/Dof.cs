namespace SimpleFEM.Core.Domain
{
    public readonly record struct Dof(int NodeId, DofType Type);
}
