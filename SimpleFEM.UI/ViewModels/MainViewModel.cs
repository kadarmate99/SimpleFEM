using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using SimpleFEM.Core.Commands;
using SimpleFEM.Core.Interfaces;
using SimpleFEM.Core.Models;
using SimpleFEM.Core.Tools;
using SimpleFEM.Data.Services;
using System.Collections.ObjectModel;
using System.Windows;

namespace SimpleFEM.UI.ViewModels
{

    public partial class MainViewModel : ObservableObject
    {
        private readonly IRepository<Node> _nodeRepository;
        private readonly IRepository<Line> _lineRepository;
        private readonly CommandManager _commandManager;
        private readonly IModelFileService _modelFileService;

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

        [ObservableProperty]
        private string _windowTitle = "SimpleFEM  - No Model Loaded";

        public IReadOnlyList<IDrawingTool> AvailableTools { get; }

        public MainViewModel(
            IRepository<Node> nodeRepository,
            IRepository<Line> lineRepository,
            IEnumerable<IDrawingTool> tools,
            CommandManager commandManager,
            IModelFileService modelFileService)
        {
            _nodeRepository = nodeRepository;
            _lineRepository = lineRepository;
            _commandManager = commandManager;
            _modelFileService = modelFileService;

            AvailableTools = tools.ToList();

            _modelFileService.ModelFileChanged += OnModelFileChanged;

            //LoadData();
            SubscribeToTools();
            SubscribeToCommandManager();
            //UpdateDocumentState();
        }

        private void OnModelFileChanged(object? sender, EventArgs e)
        {
            UpdateDocumentState();
            LoadData(); // Refresh all data from new document
        }

        private void UpdateDocumentState()
        {
            WindowTitle = _modelFileService.IsModelFileOpen
                ? $"SimpleFEM - {_modelFileService.CurrentModelFileName}"
                : "SimpleFEM - No Model Loaded";
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
        private void NewModelFile()
        {
            var dialog = new SaveFileDialog
            {
                Filter = "FEM Model Files (*.fem)|*.fem|All Files (*.*)|*.*",
                DefaultExt = ".fem",
                AddExtension = true,
                Title = "Create New FEM Model"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    _modelFileService.NewModelFile(dialog.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to create model file: {ex.Message}",
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        [RelayCommand]
        private void OpenModelFile()
        {
            var dialog = new OpenFileDialog
            {
                Filter = "FEM Model Files (*.fem)|*.fem|All Files (*.*)|*.*",
                DefaultExt = ".fem",
                AddExtension = true,
                Title = "Open New FEM Model"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    _modelFileService.OpenModelFile(dialog.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to open model file: {ex.Message}",
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        [RelayCommand]
        private async Task SaveModelFileAs()
        {
            if (!_modelFileService.IsModelFileOpen)
                return;

            var dialog = new SaveFileDialog
            {
                Filter = "FEM Model Files (*.fem)|*.fem|All Files (*.*)|*.*",
                DefaultExt = ".fem",
                AddExtension = true,
                Title = "Save FEM Model As"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    _modelFileService.SaveModelFileAs(dialog.FileName);
                    MessageBox.Show("Model File saved successfully.",
                        "Save As", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to save Model File: {ex.Message}",
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

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
