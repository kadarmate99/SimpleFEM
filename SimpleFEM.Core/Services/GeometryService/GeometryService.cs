using SimpleFEM.Core.Models;
using System.Windows;

namespace SimpleFEM.Core.Services.GeometryService
{
    public class GeometryService : IGeometryService
    {
        public double HitTolerance { get; set; } = 10.0;

        public Node? FindNodeNear(Point location, IEnumerable<Node> nodes)
        {
            return nodes.FirstOrDefault(n => IsPointNearNode(location, n));
        }

        public double DistanceToNode(Point location, Node node)
        {
            var dx = location.X - node.X;
            var dy = location.Y - node.Z;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        public bool IsPointNearNode(Point location, Node node)
        {
            return DistanceToNode(location, node) < HitTolerance;
        }
    }
}
