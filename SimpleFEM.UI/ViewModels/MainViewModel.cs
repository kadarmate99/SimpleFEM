using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SimpleFEM.Core.Interfaces;
using SimpleFEM.Core.Models;
using System.Collections.ObjectModel;
using System.Windows;

namespace SimpleFEM.UI.ViewModels
{

    public partial class MainViewModel : ObservableObject
    {
        private readonly IRepository<Node> _nodeRepository;
        private readonly IRepository<Line> _lineRepository;

        // UI State
        public ObservableCollection<NodeViewModel> Nodes { get; } = new();
        public ObservableCollection<LineViewModel> Lines { get; } = new();

        [ObservableProperty]
        private EditorTool _activeTool = EditorTool.None;

        /// <summary>
        /// State for the "Line" tool to remember the first click
        /// </summary>
        private NodeViewModel? _pendingFirstNode;

        public MainViewModel(IRepository<Node> nodeRepository, IRepository<Line> lineRepository)
        {
            _nodeRepository = nodeRepository;
            _lineRepository = lineRepository;

            LoadData();
        }

        private void LoadData()
        {
            Nodes.Clear();
            Lines.Clear();

            foreach (var node in _nodeRepository.GetAll())
            {
                Nodes.Add(new NodeViewModel(node));
            }

            foreach (var line in _lineRepository.GetAll())
            {
                Lines.Add(new LineViewModel(line));
            }
        }

        #region Commands
        [RelayCommand]
        private void SelectTool(string toolName) // TODO: Improve the tool selection button press mapping
        {
            if (Enum.TryParse(toolName, out EditorTool tool))
            {
                ActiveTool = tool;
                _pendingFirstNode = null; // Reset line state
            }
        }

        [RelayCommand]
        private void CanvasClick(Point p)
        {
            switch (ActiveTool)
            {
                case EditorTool.Node:
                    CreateNode(p);
                    break;
                case EditorTool.Line:
                    HandleLineToolClick(p);
                    break;
            }
        }

        #endregion

        // TODO: refactor this so it uses database and not direct instantiation
        private NodeViewModel CreateNode(Point location)
        {
            var nodeModel = new Node
            {
                X = location.X,
                Z = location.Y
            };

            _nodeRepository.Add(nodeModel);

            var nodeVm = new NodeViewModel(nodeModel);
            Nodes.Add(nodeVm);

            return nodeVm;
        }

        private void HandleLineToolClick(Point p)
        {
            // Is the click near an existing node? // TODO: implement real hit testing service
            var hitNode = Nodes.FirstOrDefault(n =>
                Math.Abs(n.X - p.X) < 10 && Math.Abs(n.Z - p.Y) < 10);

            // If no node exists there, create one implicitly
            if (hitNode == null)
            {
                hitNode = CreateNode(p);
            }

            if (_pendingFirstNode == null)
            {
                // First click
                _pendingFirstNode = hitNode;
            }
            else
            {
                // Second click
                if (_pendingFirstNode != hitNode)
                {
                    CreateLine(_pendingFirstNode, hitNode);
                    _pendingFirstNode = null; // Reset line state
                }
            }
        }

        // TODO: refactor this so it uses database and not direct instantiation
        private void CreateLine(NodeViewModel start, NodeViewModel end)
        {
            var lineModel = new Line
            {
                INode = start.Model,
                JNode = end.Model
            };

            _lineRepository.Add(lineModel);

            // TODO: LineViewModel instantiation creates new NodeViewModels instead of reusing existing ones; verify if this causes issues.
            var lineVm = new LineViewModel(lineModel);
            Lines.Add(lineVm);
        }
    }
}
