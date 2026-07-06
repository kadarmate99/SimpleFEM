using SimpleFEM.Core.Domain;

namespace SimpleFEM.Core.Elements;

internal sealed record ElementContext(
    IReadOnlyList<Node> Nodes,
    Material Material,
    CrossSection Section);
