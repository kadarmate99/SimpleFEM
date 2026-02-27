using CommunityToolkit.Mvvm.ComponentModel;
using SimpleFEM.UI.Navigation;

namespace SimpleFEM.UI.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _windowTitle = "SimpleFEM";

        public INavigationService Navigation { get; }

        public MainViewModel(INavigationService navigationService)
        {
            Navigation = navigationService;
        }
    }
}
