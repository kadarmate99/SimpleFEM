using CommunityToolkit.Mvvm.ComponentModel;
using SimpleFEM.Data.Services;
using SimpleFEM.UI.Navigation;

namespace SimpleFEM.UI.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly IModelFileService _modelFileService;

        [ObservableProperty]
        private string _windowTitle = "SimpleFEM";

        public INavigationService Navigation { get; }

        public MainViewModel(INavigationService navigationService, IModelFileService modelFileService)
        {
            Navigation = navigationService;
            _modelFileService = modelFileService;

            modelFileService.ModelFileChanged += OnModelFileChanged;
        }

        private void OnModelFileChanged(object? sender, EventArgs e)
        {
            WindowTitle = _modelFileService.IsModelFileOpen
                ? $"SimpleFEM - {_modelFileService.CurrentModelFileName}"
                : "SimpleFEM";
        }
    }
}
