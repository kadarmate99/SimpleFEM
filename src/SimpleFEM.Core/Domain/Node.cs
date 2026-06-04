namespace SimpleFEM.Core.Domain
{
    public class Node
    {
        public Node(int id, double x, double y)
        {
            Id = id;
            X = x;
            Y = y;
        }

        public int Id { get; }
        public double X { get; }
        public double Y { get; }
    }
}
