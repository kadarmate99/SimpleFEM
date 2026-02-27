using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using SimpleFEM.UI.ViewModels;

namespace SimpleFEM.UI.Navigation
{
    public class NavigationService : ObservableObject, INavigationService
    {
        private readonly IServiceProvider _serviceProvider;

        // not implemented as [ObservableProperty], so private setter can be ensured
        private NavigableViewModelBase? _currentViewModel; 
        public NavigableViewModelBase? CurrentViewModel
        {
            get => _currentViewModel;
            private set => SetProperty(ref _currentViewModel, value);
        }

        public NavigationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task NavigateToAsync<TViewModel>()
            where TViewModel : NavigableViewModelBase
        {
            // outgoing VM clean up
            if (_currentViewModel != null)
                await _currentViewModel.OnNavigatedFromAsync();

            var viewModel = _serviceProvider.GetRequiredService<TViewModel>();
            CurrentViewModel = viewModel;
            await viewModel.OnNavigatedToAsync();
        }
    }
}
