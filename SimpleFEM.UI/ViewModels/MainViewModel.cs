using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SimpleFEM.Core.Commands;
using SimpleFEM.Core.Interfaces;
using SimpleFEM.Core.Models;
using SimpleFEM.Core.Tools;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media.Imaging;

namespace SimpleFEM.UI.ViewModels
{

    public partial class MainViewModel : ObservableObject
    {
        private readonly IRepository<Node> _nodeRepository;
        private readonly IRepository<Line> _lineRepository;
        private readonly CommandManager _commandManager;

        // UI State
        public ObservableCollection<NodeViewModel> Nodes { get; } = new();
        public ObservableCollection<LineViewModel> Lines { get; } = new();

        [ObservableProperty]
        private IDrawingTool? _activeTool;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(UndoCommand))]
        private bool _canUndo;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(RedoCommand))]
        private bool _canRedo;

        [ObservableProperty]
        private string? _nextUndoDescription;

        [ObservableProperty]
        private string? _nextRedoDescription;

        public IReadOnlyList<IDrawingTool> AvailableTools { get; }

        public MainViewModel(
            IRepository<Node> nodeRepository,
            IRepository<Line> lineRepository,
            IEnumerable<IDrawingTool> tools,
            CommandManager commandManager)
        {
            _nodeRepository = nodeRepository;
            _lineRepository = lineRepository;
            _commandManager = commandManager;
            AvailableTools = tools.ToList();

            LoadData();
            SubscribeToTools();
            SubscribeToCommandManager();
        }

        private void SubscribeToTools()
        {
            // Tool state changes --> reload UI elements
            foreach (var tool in AvailableTools)
            {
                tool.StateChanged += (s, e) => LoadData();
            }
        }

        private void SubscribeToCommandManager()
        {
            _commandManager.CommandHistoryChanged += (s, e) =>
            {
                CanUndo = _commandManager.CanUndo;
                CanRedo = _commandManager.CanRedo;
                NextUndoDescription = _commandManager.NextUndoDescription;
                NextRedoDescription = _commandManager.NextRedoDescription;
                LoadData(); // Refresh UI after undo/redo
            };
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
            ActiveTool?.Deactivate();
            ActiveTool = AvailableTools
                .FirstOrDefault(t => t.Name.Equals(toolName, StringComparison.OrdinalIgnoreCase));
            ActiveTool?.Activate();
        }

        [RelayCommand]
        private void CanvasClick(Point location)
        {
            ActiveTool?.HandleCanvasClick(location);
        }

        [RelayCommand(CanExecute = nameof(CanUndo))]
        private void Undo()
        {
            _commandManager.Undo();
        }

        [RelayCommand(CanExecute = nameof(CanRedo))]
        private void Redo()
        {
            _commandManager.Redo();
        }

        #endregion
    }
}
