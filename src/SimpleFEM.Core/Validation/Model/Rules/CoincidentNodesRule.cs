using SimpleFEM.Core.Domain;

namespace SimpleFEM.Core.Validation.Model.Rules;

internal sealed class CoincidentNodesRule(double minNodeDistance) : IModelValidationRule
{
    public IEnumerable<ValidationError<ModelValidationErrorCode>> Validate(FemModel model)
    {
        var nodes = model.Nodes.ToList();
        double minDistanceSquared = minNodeDistance * minNodeDistance;

        for (int i = 0; i < nodes.Count - 1; i++)
        {
            for (int j = i + 1; j < nodes.Count; j++)
            {
                double dx = nodes[i].X - nodes[j].X;
                double dy = nodes[i].Y - nodes[j].Y;

                double distanceSquared = dx * dx + dy * dy;

                if (distanceSquared < minDistanceSquared)
                {
                    yield return new(
                        ModelValidationErrorCode.CoincidentNodes,
                        $"Nodes {nodes[i].Id} and {nodes[j].Id} are closer than {minNodeDistance}.",
                        (nodes[i], nodes[j]));
                }
            }
        }
    }
}
