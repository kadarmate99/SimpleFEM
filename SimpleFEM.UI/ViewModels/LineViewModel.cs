using CommunityToolkit.Mvvm.ComponentModel;
using SimpleFEM.Core.Models;

namespace SimpleFEM.UI.ViewModels
{
    public partial class LineViewModel : ObservableObject
    {
        private readonly Line _model;

        public LineViewModel(Line model)
        {
            _model = model;

            INode = new NodeViewModel(model.INode);
            JNode = new NodeViewModel(model.JNode);
        }

        public int Id => _model.Id;

        public Line Model => _model;

        public NodeViewModel INode { get; }

        public NodeViewModel JNode { get; }

        // Properties for the Line shape in XAML
        public double X1 => INode.X;
        public double Y1 => INode.Z;
        public double X2 => JNode.X;
        public double Y2 => JNode.Z;
    }
}
