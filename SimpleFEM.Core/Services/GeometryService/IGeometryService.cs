using SimpleFEM.Core.Models;
using System.Windows;

namespace SimpleFEM.Core.Services.GeometryService
{
    public interface IGeometryService
    {
        double HitTolerance { get; set; }

        Node? FindNodeNear(Point location, IEnumerable<Node> nodes);
        double DistanceToNode(Point location, Node node);
        bool IsPointNearNode(Point location, Node node);
    }
}
