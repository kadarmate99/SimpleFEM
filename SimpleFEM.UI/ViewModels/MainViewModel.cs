using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SimpleFEM.Core.Models;
using System.Collections.ObjectModel;
using System.Windows;

namespace SimpleFEM.UI.ViewModels
{

    public partial class MainViewModel : ObservableObject
    {
        public ObservableCollection<NodeViewModel> Nodes { get; } = new();
        public ObservableCollection<LineViewModel> Lines { get; } = new();

        [ObservableProperty]
        private EditorTool _activeTool = EditorTool.None;

        /// <summary>
        /// State for the "Line" tool to remember the first click
        /// </summary>
        private NodeViewModel? _pendingFirstNode;

        #region Commands
        [RelayCommand]
        private void SelectTool(string toolName) // TODO: Improve the tool selection button press mapping
        {
            if(Enum.TryParse(toolName, out EditorTool tool))
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
                Id = Nodes.Count + 1,
                X = location.X,
                Z = location.Y
            };

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

            if(_pendingFirstNode == null)
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
                Id = Lines.Count + 1,
                INode = start.Model,
                JNode = end.Model
            };

            // TODO: LineViewModel instantiation creates new NodeViewModels instead of reusing existing ones; verify if this causes issues.
            var lineVm = new LineViewModel(lineModel);  
            Lines.Add(lineVm);
        }
    }
}
