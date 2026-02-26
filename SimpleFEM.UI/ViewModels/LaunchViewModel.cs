using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using SimpleFEM.Data.Services;
using System.Windows;

namespace SimpleFEM.UI.ViewModels
{
    public partial class LaunchViewModel : ObservableObject
    {
        private readonly IModelFileService _modelFileService;
        private readonly MainWindow _mainWindow;

        public LaunchViewModel(IModelFileService modelFileService, MainWindow mainWindow)
        {
            _modelFileService = modelFileService;
            _mainWindow = mainWindow;
        }

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
                    OpenMainWindowCloseLaunchWindow();
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
                    OpenMainWindowCloseLaunchWindow();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to open model file: {ex.Message}",
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void OpenMainWindowCloseLaunchWindow()
        {
            _mainWindow.Show();
            // how to close current launch window wit this view model?
        }
    }
}
