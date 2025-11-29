using CommunityToolkit.Mvvm.ComponentModel;
using SimpleFEM.Core.Models;

namespace SimpleFEM.UI.ViewModels
{
    public partial class NodeViewModel : ObservableObject
    {
        private readonly Node _model;

        public NodeViewModel(Node model)
        {
            _model = model;
        }

        public int Id => _model.Id;

        public Node Model => _model;

        public double X
        {
            get => _model.X;
            set
            {
                if (_model.X != value)
                {
                    _model.X = value;
                    OnPropertyChanged();
                }
            }
        }
        public double Z
        {
            get => _model.Z;
            set
            {
                if (_model.Z != value)
                {
                    _model.Z = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}