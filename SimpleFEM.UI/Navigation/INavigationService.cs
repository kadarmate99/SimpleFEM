using SimpleFEM.UI.ViewModels;
using System.ComponentModel;

namespace SimpleFEM.UI.Navigation
{
    public interface INavigationService : INotifyPropertyChanged
    {
        NavigableViewModelBase? CurrentViewModel { get; }

        Task NavigateToAsync<TViewModel>() where TViewModel : NavigableViewModelBase;
    }
}
