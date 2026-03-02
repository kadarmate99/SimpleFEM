using CommunityToolkit.Mvvm.Input;
using SimpleFEM.UI.Services;

namespace SimpleFEM.UI.ViewModels
{
    public partial class LaunchViewModel : NavigableViewModelBase
    {
        private readonly IModelLoadingService _modelLoadingService;

        public LaunchViewModel(IModelLoadingService modelLoadingService)
        {
            _modelLoadingService = modelLoadingService;
        }

        [RelayCommand]
        private async Task NewModelFile() => await _modelLoadingService.NewModelAsync();

        [RelayCommand]
        private async Task OpenModelFile() => await _modelLoadingService.OpenModelAsync();
    }
}
