using SimpleFEM.Data.Services;
using SimpleFEM.UI.Navigation;
using SimpleFEM.UI.ViewModels;

namespace SimpleFEM.UI.Services
{
    public class ModelLoadingService : IModelLoadingService
    {
        private readonly IModelFileService _modelFileService;
        private readonly IDialogService _dialogService;
        private readonly INavigationService _navigationService;

        private const string ModelFilter = "FEM Model Files (*.fem)|*.fem|All Files (*.*)|*.*";
        private const string FileExtension = ".fem";

        public ModelLoadingService(
            IModelFileService modelFileService,
            IDialogService dialogService,
            INavigationService navigationService)
        {
            _modelFileService = modelFileService;
            _dialogService = dialogService;
            _navigationService = navigationService;
        }

        /// <inheritdoc/>
        public async Task NewModelAsync()
        {
            var filePath = _dialogService.ShowSaveFileDialog(
                "Create New FEM Model",
                ModelFilter,
                FileExtension);

            if (filePath is null) // User cancelled
                return;

            try
            {
                _modelFileService.NewModelFile(filePath);
                await _navigationService.NavigateToAsync<CanvasViewModel>();
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Failed to create model file: {ex.Message}");
            }
        }

        /// <inheritdoc/>
        public async Task OpenModelAsync()
        {

            var filePath = _dialogService.ShowOpenFileDialog(
                "Open New FEM Model",
                ModelFilter,
                FileExtension);

            if (filePath is null) // User cancelled
                return;

            try
            {
                _modelFileService.OpenModelFile(filePath);
                await _navigationService.NavigateToAsync<CanvasViewModel>();
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Failed to open model file: {ex.Message}");
            }
        }

        /// <inheritdoc/>
        public async Task SaveModelAsAsync()
        {
            if (!_modelFileService.IsModelFileOpen)
                return;

            var filePath = _dialogService.ShowSaveFileDialog(
                "Save FEM Model As",
                ModelFilter,
                FileExtension);

            if (filePath is null) // User cancelled
                return;

            try
            {
                _modelFileService.SaveModelFileAs(filePath);
                _dialogService.ShowInfo("Model File saved successfully.");
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Failed to save Model File: {ex.Message}");
            }
        }
    }
}
