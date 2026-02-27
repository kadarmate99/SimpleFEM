using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using SimpleFEM.Data.Services;
using SimpleFEM.UI.Navigation;
using System.Windows;

namespace SimpleFEM.UI.ViewModels
{
    public partial class LaunchViewModel : NavigableViewModelBase
    {
        private readonly IModelFileService _modelFileService;
        private readonly INavigationService _navigationService;

        public LaunchViewModel(IModelFileService modelFileService, INavigationService navigationService)
        {
            _modelFileService = modelFileService;
            _navigationService = navigationService;
        }

        [RelayCommand]
        private async Task NewModelFile()
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
                    await _navigationService.NavigateToAsync<CanvasViewModel>();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to create model file: {ex.Message}",
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        [RelayCommand]
        private async Task OpenModelFile()
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
                    await _navigationService.NavigateToAsync<CanvasViewModel>();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to open model file: {ex.Message}",
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
